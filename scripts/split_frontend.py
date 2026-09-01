from pathlib import Path
import subprocess
import urllib.request

REPO = "abdulRahman-muhamed07/nexus-erp"
BRANCH = "feature/frontend-split-final"
RAW = f"https://raw.githubusercontent.com/{REPO}/master/"


def get_inline_script(html: str) -> tuple[str, int, int]:
    search = 0
    while True:
        start = html.find("<script", search)
        if start < 0:
            raise RuntimeError("Inline JavaScript block not found")
        open_end = html.find(">", start)
        if open_end < 0:
            raise RuntimeError("Malformed script tag")
        close = html.find("</script>", open_end + 1)
        if close < 0:
            raise RuntimeError("Malformed script block")
        tag = html[start : open_end + 1].lower()
        if "src=" not in tag:
            return html[open_end + 1 : close], start, close + len("</script>")
        search = close + len("</script>")


def main() -> None:
    work = Path(".split-work")
    work.mkdir(exist_ok=True)
    source = work / "nexus-erp.html"
    workflow_source = work / "backend-ci.yml"

    urllib.request.urlretrieve(RAW + "nexus-erp.html", source)
    urllib.request.urlretrieve(RAW + ".github/workflows/backend-ci.yml", workflow_source)

    original = source.read_text(encoding="utf-8")

    style_open = original.find("<style")
    if style_open < 0:
        raise RuntimeError("Inline CSS block not found")
    style_start = original.find(">", style_open) + 1
    style_close = original.find("</style>", style_start)
    if style_start <= 0 or style_close < 0:
        raise RuntimeError("Malformed CSS block")

    css = original[style_start:style_close].strip() + "\n"
    html = original[:style_open] + '<link rel="stylesheet" href="../css/nexus-erp.css">' + original[style_close + len("</style>") :]

    js, js_start, js_end = get_inline_script(html)
    html = html[:js_start] + html[js_end:]
    html = html.replace('</body>', '<script src="../js/nexus-erp.js"></script>\n</body>', 1)

    Path("front/html").mkdir(parents=True, exist_ok=True)
    Path("front/css").mkdir(parents=True, exist_ok=True)
    Path("front/js").mkdir(parents=True, exist_ok=True)

    Path("front/html/nexus-erp.html").write_text(html.strip() + "\n", encoding="utf-8")
    Path("front/css/nexus-erp.css").write_text(css, encoding="utf-8")
    Path("front/js/nexus-erp.js").write_text(js.strip() + "\n", encoding="utf-8")

    Path("nexus-erp.html").unlink(missing_ok=True)
    for old in Path("front/js").glob("nexus-erp-*.js"):
        old.unlink()
    for keep in ("front/html/.gitkeep", "front/css/.gitkeep", "front/js/.gitkeep"):
        Path(keep).unlink(missing_ok=True)

    # Restore the repository's normal CI workflow in the generated commit.
    Path(".github/workflows/backend-ci.yml").write_text(workflow_source.read_text(encoding="utf-8"), encoding="utf-8")

    # Remove the temporary splitter and scratch files before committing.
    Path("scripts/split_frontend.py").unlink(missing_ok=True)
    for p in (work / "nexus-erp.html", work / "backend-ci.yml"):
        p.unlink(missing_ok=True)
    work.rmdir()

    subprocess.run(["git", "add", "-A"], check=True)
    subprocess.run(["git", "config", "user.name", "github-actions[bot]"], check=True)
    subprocess.run(["git", "config", "user.email", "41898282+github-actions[bot]@users.noreply.github.com"], check=True)
    subprocess.run(["git", "commit", "-m", "chore: split frontend assets"], check=True)
    subprocess.run(["git", "push", "origin", f"HEAD:{BRANCH}"], check=True)


if __name__ == "__main__":
    main()
