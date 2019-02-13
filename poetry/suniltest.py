#from pyrym import ModelReader, WordModel
from BanglaModel import ModelReader, BanglaModel


reader = ModelReader("sunil.mdl")
reader.read()
print(reader.nextline("চলে"))
