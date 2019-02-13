from pyrym import ModelReader, WordModel

reader = ModelReader("sunil.mdl")
reader.read()
print(reader.nextline("চলে", 5))
