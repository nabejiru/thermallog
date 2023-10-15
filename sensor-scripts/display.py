from board import SCL, SDA
import busio

from PIL import Image, ImageDraw, ImageFont
import adafruit_ssd1306

class Display:
    oled = None
    draw = None
    font = None
    image = None

    def __init__(self):
        # Create the I2C interface.
        # Create the SSD1306 OLED class.
        i2c = busio.I2C(SCL, SDA)
        self.oled = adafruit_ssd1306.SSD1306_I2C(128, 32, i2c)

        # Create blank image for drawing.
        self.image = Image.new("1", (self.oled.width, self.oled.height))
        # Get drawing object to draw on image.
        self.draw = ImageDraw.Draw(self.image)
        self.font = ImageFont.truetype("/usr/share/fonts/opentype/noto/NotoSansCJK-Regular.ttc", 12)

    def clear(self):
        # Clear display.
        self.oled.fill(0)

    def put(self, text, lineNo):
        self.draw.text((0, lineNo * 12 + 4 * lineNo), text, font=self.font, fill=255)
        # Display image
        self.oled.image(self.image)

    def show(self):
        self.oled.show()

