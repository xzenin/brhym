from numpy import array
from keras.preprocessing.text import Tokenizer
from keras.utils import to_categorical
from keras.preprocessing.sequence import pad_sequences
from keras.models import Sequential
from keras.layers import Dense
from keras.layers import LSTM
from keras.layers import Embedding
import pickle

class BanglaModel:
    max_length = 0
    tokenizer = []
    model = []
    sequences =[]

class WordModel:
    max_length = 0
    tokenizer = []
    model = []
    sequences =[]

class ModelReader :
    bmodel = []
    filename = "default.mdl"
    def __init__(self,filetoread):
        self.filename = filetoread

    def read(self): 
        with open(self.filename, "rb") as f:
            print("Reading file: " + self.filename)
            self.bmodel = pickle.load(f)
            print("File read: " + self.filename)
        return self.bmodel

    def write(self, model): 
        with open(self.filename, "wb") as f:
            self.bmodel = model
            print("Writing file: " + self.filename)
            pickle.dump(self.bmodel, f)
            print("File written: " + self.filename)

    # generate a sequence from a language model
    def generate_seq(self, model, tokenizer, max_length, seed_text, n_words):
        in_text = seed_text
        # generate a fixed number of words
        for _ in range(n_words):
            # encode the text as integer
            encoded = tokenizer.texts_to_sequences([in_text])[0]
            # pre-pad sequences to a fixed length
            encoded = pad_sequences([encoded], maxlen=max_length, padding='pre')
            # predict probabilities for each word
            yhat = model.predict_classes(encoded, verbose=0)
            # map predicted word index to word
            out_word = ''
            for word, index in tokenizer.word_index.items():
                if index == yhat:
                    out_word = word
                    break
            # append to input
            in_text += ' ' + out_word
        return in_text

    def nextline(self, start, nums):
        model = self.bmodel.model
        tokenizer = self.bmodel.tokenizer
        max_length = self.bmodel.max_length
        line = "কিছু পাওয়া যায়নি । ধন্যবাদ । "
        try:
            line = self.generate_seq(model, tokenizer, max_length-1, start, nums)
        except Exception as ex:
            print("Some Error, or word not found in the context.", ex.args[0])	 
        return line
