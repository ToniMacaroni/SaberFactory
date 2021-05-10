import glob

files = glob.glob(r"**\*.bsml", recursive=True)

for f in files:
    stream = open(f, "r")
    content = stream.read()
    stream.close()
    stream = open(f, "w")

    content = content.replace("<custom-icon-btn", "<sui.icon-button")
    content = content.replace("</custom-icon-btn", "</sui.icon-button")

    stream.write(content)
    stream.close()