import glob

files = glob.glob(r"**\*.bsml", recursive=True)

for f in files:
    print(f)