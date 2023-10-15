from board import SCL, SDA
import busio
import adafruit_ssd1306
# Create the I2C interface.
# Create the SSD1306 OLED class.
i2c = busio.I2C(SCL, SDA)
oled = adafruit_ssd1306.SSD1306_I2C(128, 32, i2c)

# Clear display.
oled.fill(0)
oled.show()