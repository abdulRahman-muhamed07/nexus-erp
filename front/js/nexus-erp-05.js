}
function renderDraft(){
  const wrap=$('#of-items');
  wrap.innerHTML=draft.items.map((it,idx)=>{
    const opts='<option value="">Select product…</option>'+db.products.map(p=>`<option value="${p.id}" ${it.productId===p.id?'selected':''}>${esc(p.name)} — ${money2(p.price)}</option>`).join('');
    const p=db.products.find(x=>x.id===it.productId);
    return `<div class="draft-row"><select data-di="${idx}" data-f="productId">${opts}</select><input type="number" min="1" max="999" value="${it.qty}" data-di="${idx}" data-f="qty"><span class="draft-line">${p?money2(p.price*it.qty):'—'}</span><button type="button" class="icon-btn sm danger" data-del="${idx}"><i class="fa-regular fa-trash-can"></i></button></div>`;
  }).join('');
  const cfg=db.config||{taxRate:8,shippingFee:25,freeShipOver:1000};
  const sub=draft.items.reduce((s,it)=>{const p=db.products.find(x=>x.id===it.productId);return s+(p?p.price*it.qty:0);},0);
  const discPct=Math.max(0,Math.min(100,draft.discPct||0));
  const disc=sub*discPct/100,taxable=sub-disc;
  const tax=taxable*cfg.taxRate/100,ship=sub>cfg.freeShipOver||sub===0?0:cfg.shippingFee;
  $('#of-totals').innerHTML=`<div>Subtotal<span>${money2(sub)}</span></div>${discPct?`<div>Discount (${discPct}%)<span style="color:var(--red)">-${money2(disc)}</span></div>`:''}<div>Tax (${cfg.taxRate}%)<span>${money2(tax)}</span></div><div>Shipping<span>${ship?money2(ship):'Free'}</span></div><div class="tt">Total<span>${money2(taxable+tax+ship)}</span></div>`;
  $$('#of-items [data-di]').forEach(el=>el.onchange=()=>{const i=+el.dataset.di,f=el.dataset.f;draft.items[i][f]=f==='qty'?Math.max(1,Math.min(999,+el.value||1)):el.value;renderDraft();});
  $$('#of-items [data-del]').forEach(b=>b.onclick=()=>{draft.items.splice(+b.dataset.del,1);if(!draft.items.length)draft.items.push({productId:'',qty:1});renderDraft();});
}
function saveOrder(){
  const customerId=$('#of-cust').value,date=$('#of-date').value,status=$('#of-status').value;
  const items=draft.items.filter(it=>it.productId&&it.qty>0).map(it=>{const p=db.products.find(x=>x.id===it.productId);return {productId:p.id,name:p.name,qty:it.qty,price:p.price};});
  if(!items.length){toast('Add at least one line item','fa-triangle-exclamation','warn');return;}
  const cfg=db.config||{taxRate:8,shippingFee:25,freeShipOver:1000};
  const sub=items.reduce((s,i)=>s+i.qty*i.price,0);
  const discPct=Math.max(0,Math.min(100,+($('#of-disc').value)||0));
  const disc=sub*discPct/100,taxable=sub-disc;
  const tax=taxable*cfg.taxRate/100,ship=sub>cfg.freeShipOver||sub===0?0:cfg.shippingFee,total=taxable+tax+ship;
  const cust=db.customers.find(c=>c.id===customerId);
  if(draft.mode!=='quote'&&cust){let base=null;if(draft.editingId){const eo=db.orders.find(o=>o.id===draft.editingId);if(eo)base=Math.max(0,customerOutstanding(cust)-Math.max(0,orderNet(eo)-orderPaid(eo)));}const block=creditBlock(cust,total,base);if(block){toast(block,'fa-ban','error');audit('fa-ban','Order blocked','Orders',cust.company,block);return;}}
  if(draft.mode==='quote'){
    const payload={customerId,customerName:cust.company,items,sub,tax,ship,total,discPct,status,date:new Date(date+'T12:00:00').toISOString()};
    if(draft.editingId){
      const q=db.quotes.find(x=>x.id===draft.editingId);
      if(q.status==='Converted'){toast('Converted quotes cannot be edited','fa-lock','warn');return;}
      Object.assign(q,payload);
      audit('fa-pen-to-square','Quote updated','Quotes',q.id,`${money2(total)} · ${cust.company}`);
      toast('Quote '+q.id+' updated');
    }else{
      const id='QT-'+(db.seq.quote++);
      db.quotes.unshift({id,...payload});
      addActivity('fa-file-lines',`Quote ${id} created for ${cust.company} — ${money(total)}`);
      audit('fa-file-lines','Quote created','Quotes',id,`${money2(total)} · ${cust.company}`);
      toast('Quote '+id+' created');
    }
    save();closeModal();ordersTab='quotes';navigate('orders');return;
  }
  if(draft.editingId){
    const old=db.orders.find(o=>o.id===draft.editingId);
    const prev=old.status;
    if(prev!=='Cancelled')applyStock(old,1,'Order Edit',old.id);
    Object.assign(old,{customerId,customerName:cust.company,items,sub,tax,ship,total,discPct,status,date:new Date(date+'T12:00:00').toISOString()});
    if(status!=='Cancelled')applyStock(old,-1,'Sale',old.id);
    audit('fa-pen-to-square','Order updated','Orders',old.id,`${prev} → ${status} · ${money2(total)}`);
    toast('Order '+old.id+' updated');
  }else{
    const id='ORD-'+(db.seq.order++);
    const o={id,customerId,customerName:cust.company,items,sub,tax,ship,total,discPct,status,date:new Date(date+'T12:00:00').toISOString(),payments:[],returns:[]};
    db.orders.unshift(o);
    if(status!=='Cancelled')applyStock(o,-1,'Sale',o.id);
    addActivity('fa-cart-shopping',`New order ${id} from ${cust.company} — ${money(total)}`);
    audit('fa-cart-shopping','Order created','Orders',id,`${money2(total)} · ${cust.company}`);
    pushNotif('fa-cart-shopping','New order received',`${id} · ${cust.company} · ${money(total)}`);
    toast('Order '+id+' created');
  }
  save();closeModal();navigate('orders');
}
function duplicateOrder(id){
  const src=db.orders.find(o=>o.id===id);if(!src)return;
  const nid='ORD-'+(db.seq.order++);
  const copy={id:nid,customerId:src.customerId,customerName:src.customerName,items:src.items.map(i=>({...i})),sub:src.sub,tax:src.tax,ship:src.ship,total:src.total,discPct:src.discPct||0,status:'Pending',date:new Date().toISOString(),payments:[],returns:[]};
  db.orders.unshift(copy);
  applyStock(copy,-1,'Sale',copy.id);
  addActivity('fa-copy',`Order ${nid} duplicated from ${src.id} — ${money(copy.total)}`);
  audit('fa-copy','Order duplicated','Orders',nid,`Copy of ${src.id} · ${money2(copy.total)}`);
  pushNotif('fa-copy','Order duplicated',`${nid} copied from ${src.id} · ${money(copy.total)}`);
  save();toast('Order '+nid+' created from '+src.id,'fa-copy');rerender();
}
function openOrderView(id){
  const o=db.orders.find(x=>x.id===id);if(!o)return;
  const paid=orderPaid(o),refunded=orderRefunded(o),net=orderNet(o),bal=Math.max(0,net-paid);
  const inv=db.invoices.find(i=>i.orderId===o.id);
  const payRows=(o.payments||[]).slice().reverse().map(p=>`<div class="act-item"><span class="act-ico" style="background:rgba(16,185,129,.12);color:var(--green)"><i class="fa-solid fa-money-bill"></i></span><div><p><b>${money2(p.amount)}</b> · ${esc(p.method)}</p><time>${fmtDate(p.date)}</time></div></div>`).join('')||'<div class="empty" style="padding:14px"><i class="fa-regular fa-hourglass-half"></i>No payments recorded yet</div>';
  const retRows=(o.returns||[]).slice().reverse().map(r=>`<div class="act-item"><span class="act-ico" style="background:rgba(244,63,94,.12);color:var(--red)"><i class="fa-solid fa-rotate-left"></i></span><div><p><b>${money2(r.amount)}</b> · ${r.items.map(i=>i.qty+' × '+i.name).join(', ')}</p><time>${esc(r.reason)} · ${fmtDate(r.date)}</time></div></div>`).join('')||'';
  openModal(`<div class="modal-head"><div><h3>${o.id}</h3><p>Placed ${fmtDT(o.date)}${inv?' · <a href="#" style="color:var(--primary)" id="goInv">'+inv.id+'</a>':''}</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <div class="modal-body">
    <div class="ov-grid"><div><span>Customer</span><b>${esc(o.customerName)}</b></div><div><span>Status</span>${badge(o.status)}</div><div><span>Items</span><b>${o.items.reduce((s,i)=>s+i.qty,0)}</b></div><div><span>Payment</span>${badge(payStatus(o))}</div></div>
    <div class="of-totals">${o.discPct?`<div>Discount (${o.discPct}%)<span style="color:var(--red)">-${money2(o.sub*o.discPct/100)}</span></div>`:''}<div>Total<span>${money2(o.total)}</span></div>${refunded?`<div>Refunded<span style="color:var(--red)">-${money2(refunded)}</span></div>`:''}<div>Paid<span style="color:var(--green)">${money2(paid)}</span></div><div>Balance${bal>0?' <span class="badge b-red" style="margin-left:4px">Due</span>':''}<span>${money2(bal)}</span></div></div>
    <table class="tbl mini" style="margin-top:8px"><thead><tr><th>Item</th><th>Qty</th><th>Price</th><th class="ta-r">Amount</th></tr></thead><tbody>${o.items.map(i=>`<tr><td>${esc(i.name)}</td><td>${i.qty}</td><td>${money2(i.price)}</td><td class="ta-r">${money2(i.qty*i.price)}</td></tr>`).join('')}</tbody></table>
    <div class="items-label" style="margin-top:14px"><span>Payments</span></div>
    <div style="border:1px solid var(--border);border-radius:12px;padding:2px 14px">${payRows}</div>
    ${retRows?`<div class="items-label" style="margin-top:14px"><span>Returns</span></div><div style="border:1px solid rgba(244,63,94,.3);border-radius:12px;padding:2px 14px">${retRows}</div>`:''}
  </div>
  <div class="modal-foot"><label class="inline-label">Status<select id="ov-status">${['Pending','Processing','Shipped','Delivered','Cancelled'].map(s=>`<option ${s===o.status?'selected':''}>${s}</option>`).join('')}</select></label>${can('create')&&!inv?`<button class="btn btn-ghost btn-sm" id="ov-inv"><i class="fa-solid fa-file-invoice-dollar"></i> Create Invoice</button>`:''}${bal>0.005&&can('edit')?`<button class="btn btn-ghost btn-sm" id="ov-pay"><i class="fa-solid fa-money-bill"></i> Record Payment</button>`:''}${o.status==='Delivered'&&can('edit')?`<button class="btn btn-ghost btn-sm" id="ov-ret" style="color:var(--red)"><i class="fa-solid fa-rotate-left"></i> Record Return</button>`:''}<div class="spacer"></div><button class="btn btn-ghost modal-close">Close</button><button class="btn btn-primary" id="ov-save">Update Status</button></div>`,680);
  const gi=$('#goInv');if(gi)gi.onclick=e=>{e.preventDefault();closeModal();navigate('finance');};
  const ip=$('#ov-inv');if(ip)ip.onclick=()=>{const nv=createInvoiceForOrder(o);audit('fa-file-invoice-dollar','Invoice created','Finance',nv.id,'From order '+o.id);save();toast('Invoice '+nv.id+' created');closeModal();rerender();};
  const pp=$('#ov-pay');if(pp)pp.onclick=()=>openPaymentModal(o);
  const rt=$('#ov-ret');if(rt)rt.onclick=()=>openReturnModal(o);
  $('#ov-save').onclick=()=>{
    const ns=$('#ov-status').value,had=db.invoices.some(i=>i.orderId===o.id),prev=o.status;
    changeStatus(o,ns);
    audit('fa-rotate','Status updated','Orders',o.id,`${prev} → ${ns}`);
    if(ns==='Delivered'){const nv=createInvoiceForOrder(o);if(nv&&!had){pushNotif('fa-file-invoice-dollar','Invoice generated',`${nv.id} for ${o.id} · ${money(nv.amount)}`);addActivity('fa-file-invoice-dollar',`Invoice ${nv.id} generated from ${o.id}`);audit('fa-file-invoice-dollar','Invoice auto-generated','Finance',nv.id,'From Delivered order '+o.id);}}
    save();toast(o.id+' status updated');closeModal();rerender();
  };
}
function openReturnModal(o){
  openModal(`<div class="modal-head"><div><h3>Record Return — ${o.id}</h3><p>Restore stock and record a refund</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="retForm" class="modal-body">
    <div class="items-label"><span>Items to return</span></div>
    <div id="ret-items"></div>
    <label>Reason<select id="ret-reason"><option>Damaged Item</option><option>Incorrect Item</option><option>Customer Return</option><option>Quality Issue</option><option>Other</option></select></label>
    <div class="of-totals"><div>Refund Amount<span id="ret-amt">—</span></div></div>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-danger" id="ret-save"><i class="fa-solid fa-rotate-left"></i> Record Return</button></div>`,520);
  const wrap=$('#ret-items');
  wrap.innerHTML=o.items.map((it,idx)=>`<div class="draft-row"><span style="flex:1;min-width:0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;font-size:13px">${esc(it.name)}</span><input type="number" min="0" max="${it.qty}" value="0" data-retq="${idx}" style="width:80px"><span class="draft-line">${money2(it.price)} ea</span></div>`).join('')||'<div class="empty">No line items</div>';
  const calc=()=>{let amt=0;$$('#ret-items [data-retq]').forEach(el=>{const idx=+el.dataset.retq;amt+=Math.max(0,+el.value||0)*o.items[idx].price;});$('#ret-amt').textContent=money2(amt);return amt;};
  $$('#ret-items [data-retq]').forEach(el=>el.oninput=calc);
  calc();
  $('#retForm').onsubmit=e=>{e.preventDefault();$('#ret-save').click();};
  $('#ret-save').onclick=()=>{
    const lines=[];
    $$('#ret-items [data-retq]').forEach(el=>{const idx=+el.dataset.retq,qty=Math.max(0,+el.value||0);if(qty>0)lines.push({productId:o.items[idx].productId,name:o.items[idx].name,qty:Math.min(qty,o.items[idx].qty),price:o.items[idx].price});});
    if(!lines.length){toast('Select at least one item to return','fa-triangle-exclamation','warn');return;}
    recordReturn(o,lines,$('#ret-reason').value);
    save();toast('Return recorded — stock restored','fa-rotate-left');closeModal();openOrderView(o.id);
  };
}
function recordReturn(o,lines,reason){
  const amount=lines.reduce((s,l)=>s+l.qty*l.price,0);
  lines.forEach(l=>{const p=db.products.find(x=>x.id===l.productId);if(p)p.stock+=l.qty;logMovement(l.productId,l.qty,'Return',o.id);});
  o.returns=o.returns||[];
  o.returns.push({id:'r'+Date.now(),date:new Date().toISOString(),items:lines.map(l=>({name:l.name,qty:l.qty,price:l.price})),reason,amount});
  audit('fa-rotate-left','Return recorded','Orders',o.id,`${money2(amount)} refunded · ${lines.reduce((s,l)=>s+l.qty,0)} unit(s)`);
  addActivity('fa-rotate-left',`Return recorded for ${o.id} — ${money2(amount)} refunded`);
  pushNotif('fa-rotate-left','Return recorded',`${money2(amount)} refunded for ${o.id}`);
  return amount;
}
function openPaymentModal(o){
  const bal=Math.max(0,orderNet(o)-orderPaid(o));
  openModal(`<div class="modal-head"><div><h3>Record Payment — ${o.id}</h3><p>Outstanding balance ${money2(bal)}</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="payForm" class="modal-body">
    <label>Amount ($)<input type="number" min="0.01" step="0.01" id="pay-amt" value="${bal.toFixed(2)}"></label>
    <label>Method<select id="pay-method"><option>Card</option><option>Bank Transfer</option><option>Cash</option><option>Cheque</option></select></label>
    <label>Payment Date<input type="date" id="pay-date" value="${new Date().toISOString().slice(0,10)}"></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="pay-save"><i class="fa-solid fa-check"></i> Record Payment</button></div>`,480);
  $('#payForm').onsubmit=e=>{e.preventDefault();$('#pay-save').click();};
  $('#pay-save').onclick=()=>{
    const amt=+$('#pay-amt').value||0;
    if(amt<=0){toast('Enter a valid amount','fa-triangle-exclamation','warn');return;}
    o.payments=o.payments||[];
    const amt2=+Math.min(amt,bal).toFixed(2);
    o.payments.push({id:'p'+Date.now(),amount:amt2,method:$('#pay-method').value,date:new Date($('#pay-date').value+'T12:00:00').toISOString()});
    audit('fa-money-bill','Payment recorded','Orders',o.id,`${money2(amt2)} via ${$('#pay-method').value}`);
    addActivity('fa-money-bill',`Payment of ${money2(amt2)} received for ${o.id}`);
    pushNotif('fa-money-bill','Payment received',`${money2(amt)} for ${o.id} · ${o.customerName}`);
    const inv=db.invoices.find(i=>i.orderId===o.id);
    if(inv&&orderPaid(o)>=o.total-0.005){inv.status='Paid';inv.paidOn=new Date().toISOString();}
    save();toast('Payment recorded','fa-circle-check');closeModal();openOrderView(o.id);
  };
}
function openQuoteView(id){
  const q=db.quotes.find(x=>x.id===id);if(!q)return;
  const converted=q.status==='Converted';
  openModal(`<div class="modal-head"><div><h3>${q.id}</h3><p>Prepared ${fmtDT(q.date)}</p></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <div class="modal-body">
    <div class="ov-grid"><div><span>Customer</span><b>${esc(q.customerName)}</b></div><div><span>Status</span>${badge(q.status)}</div><div><span>Items</span><b>${q.items.reduce((s,i)=>s+i.qty,0)}</b></div><div><span>Total</span><b>${money2(q.total)}</b></div></div>
    <table class="tbl mini"><thead><tr><th>Item</th><th>Qty</th><th>Price</th><th class="ta-r">Amount</th></tr></thead><tbody>${q.items.map(i=>`<tr><td>${esc(i.name)}</td><td>${i.qty}</td><td>${money2(i.price)}</td><td class="ta-r">${money2(i.qty*i.price)}</td></tr>`).join('')}</tbody></table>
    <div class="of-totals"><div>Subtotal<span>${money2(q.sub)}</span></div><div>Tax<span>${money2(q.tax)}</span></div><div>Shipping<span>${q.ship?money2(q.ship):'Free'}</span></div><div class="tt">Total<span>${money2(q.total)}</span></div></div>
  </div>
  <div class="modal-foot">${!converted?`<label class="inline-label">Status<select id="qv-status">${['Draft','Sent','Approved'].map(s=>`<option ${s===q.status?'selected':''}>${s}</option>`).join('')}</select></label>`:''}<div class="spacer"></div>${!converted&&can('create')?`<button class="btn btn-primary" id="qv-conv"><i class="fa-solid fa-arrow-right"></i> Convert to Order</button>`:''}<button class="btn btn-ghost modal-close">Close</button></div>`,620);
  const vs=$('#qv-status');
  if(vs){vs.addEventListener('change',()=>{q.status=vs.value;audit('fa-rotate','Quote status updated','Quotes',q.id,q.status);save();toast('Quote status updated');});}
  const cv=$('#qv-conv');
  if(cv)cv.onclick=()=>{convertQuote(q.id);save();closeModal();rerender();};
}
function openProductModal(p){
  const editing=!!p;
  const cats=['Electronics','Accessories','Furniture','Office'];
  openModal(`<div class="modal-head"><div><h3>${editing?'Edit Product':'Add Product'}</h3></div><button class="icon-btn modal-close"><i class="fa-solid fa-xmark"></i></button></div>
  <form id="pForm" class="modal-body">
    <label>Product Name<input id="pp-name" required value="${editing?esc(p.name):''}"></label>
    <div class="frow2"><label>SKU<input id="pp-sku" required value="${editing?esc(p.sku):'AUTO'}"></label><label>Category<select id="pp-cat">${cats.map(c=>`<option ${editing&&p.category===c?'selected':''}>${c}</option>`).join('')}</select></label></div>
    <div class="frow2"><label>Supplier<select id="pp-sup">${db.suppliers.map(s=>`<option ${editing&&p.supplier===s.name?'selected':''}>${esc(s.name)}</option>`).join('')}</select></label><label>Reorder Level<input type="number" min="0" id="pp-re" required value="${editing?p.reorder:20}"></label></div>
    <div class="frow2"><label>Sale Price ($)<input type="number" step="0.01" min="0" id="pp-price" required value="${editing?p.price:''}"></label><label>Unit Cost ($)<input type="number" step="0.01" min="0" id="pp-cost" required value="${editing?p.cost:''}"></label></div>
    <label>Stock on Hand<input type="number" min="0" id="pp-stock" required value="${editing?p.stock:0}"></label>
  </form>
  <div class="modal-foot"><div class="spacer"></div><button class="btn btn-ghost modal-close">Cancel</button><button class="btn btn-primary" id="pp-save"><i class="fa-solid fa-check"></i> ${editing?'Save Changes':'Add Product'}</button></div>`,560);
  $('#pForm').onsubmit=e=>{e.preventDefault();$('#pp-save').click();};
  $('#pp-save').onclick=()=>{
    const data={name:$('#pp-name').value.trim(),sku:$('#pp-sku').value.trim()==='AUTO'?'SKU-'+ri(1000,9999):$('#pp-sku').value.trim(),category:$('#pp-cat').value,supplier:$('#pp-sup').value,price:+$('#pp-price').value,cost:+$('#pp-cost').value,stock:Math.max(0,+$('#pp-stock').value),reorder:Math.max(0,+$('#pp-re').value)};
    if(!data.name||isNaN(data.price))return;
    if(editing){const diff=data.stock-p.stock;Object.assign(p,data);if(diff)logMovement(p.id,diff,'Adjustment','Product Edit');audit('fa-pen-to-square','Product updated','Inventory',data.name);}else{db.products.push({id:'PRD-'+(db.seq.product++),...data});addActivity('fa-box','New product added: '+data.name);audit('fa-box','Product created','Inventory',data.name,`${money2(data.price)} · ${data.category}`);}
    save();closeModal();toast(editing?'Product updated':'Product added');rerender();
  };
