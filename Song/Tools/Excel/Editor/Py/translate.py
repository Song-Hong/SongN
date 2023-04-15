from googletrans import Translator
import sys

text = ''
target = ''

if len(sys.argv) >= 3:  # 改为大于等于 3
    text = sys.argv[1]
    target = sys.argv[2]
else:
    exit()

text = str(text).replace('_', ' ')

result = Translator().translate(text, dest=str(target))

print(result.text)
