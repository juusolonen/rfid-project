import time
import RPi.GPIO as GPIO

class ToolShelf:

    _instance = None
    outgoing = [False, False, False, False, False]
    incoming = [False, False, False, False, False]
    snapshot = None
    reading = False
    admin_mode = False
    admin_tagid = None
    admin_id = None
    user_in = None

    def __new__(cls, *args, **kwargs):
        if not cls._instance:
            cls._instance = super(ToolShelf, cls).__new__(cls, *args, **kwargs)
        return cls._instance

    def __init__(self):
        if not hasattr(self, "initialized"):
            GPIO.setmode(GPIO.BOARD)
            self.INPUT_PIN = 33      # from 74HC151 pin5 violet wire
            GPIO.setup(self.INPUT_PIN, GPIO.IN, GPIO.PUD_DOWN)
            self.SELECT_SWITCH0 = 18 # from 74HC151 pin11 blue wire
            GPIO.setup(self.SELECT_SWITCH0, GPIO.OUT)
            self.SELECT_SWITCH1 = 29 # from 74HC151 pin10 green wire
            GPIO.setup(self.SELECT_SWITCH1, GPIO.OUT)
            self.SELECT_SWITCH2 = 31 # from 74HC151 pin9 yellow wire
            GPIO.setup(self.SELECT_SWITCH2, GPIO.OUT)

            # Set up GPIO18 as PWM output for Buzzer
            GPIO.setup(32, GPIO.OUT)
            try:
                self.pwm = GPIO.PWM(32, 2000)
            except RuntimeError:
                 # If PWM object already exists, stop it and create a new one
                GPIO.cleanup(32)
                GPIO.setup(32, GPIO.OUT)
                self.pwm = GPIO.PWM(32, 2000)



    def read_shelf(self):
        if self.reading:
            return False
        
        self.reading = True

        result = [None, None, None, None, None]

        #read first 6 inputs from 74HC151 000, 001, 010, 011 and 100
        GPIO.output(self.SELECT_SWITCH0, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH1, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH2, GPIO.LOW)
        time.sleep(0.25)
        #read and print first
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            result[0] = True
        else:
            result[0] = False
        GPIO.output(self.SELECT_SWITCH0, GPIO.HIGH)
        GPIO.output(self.SELECT_SWITCH1, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH2, GPIO.LOW)
        time.sleep(0.25)
        # read and print second
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            result[1] = True
        else:
            result[1] = False
        GPIO.output(self.SELECT_SWITCH0, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH1, GPIO.HIGH)
        GPIO.output(self.SELECT_SWITCH2, GPIO.LOW)
        time.sleep(0.25)
        # read and print second
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            result[2] = True
        else:
            result[2] = False
        GPIO.output(self.SELECT_SWITCH0, GPIO.HIGH)
        GPIO.output(self.SELECT_SWITCH1, GPIO.HIGH)
        GPIO.output(self.SELECT_SWITCH2, GPIO.LOW)
        time.sleep(0.25)
        # read and print second
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            result[3] = True
        else:
            result[3] = False
        GPIO.output(self.SELECT_SWITCH0, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH1, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH2, GPIO.HIGH)
        time.sleep(0.25)
        # read and print second
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            result[4] = True
        else:
            result[4] = False

        self.reading = False
        return result

    def read_door(self):
        GPIO.output(self.SELECT_SWITCH0, GPIO.HIGH)
        GPIO.output(self.SELECT_SWITCH1, GPIO.LOW)
        GPIO.output(self.SELECT_SWITCH2, GPIO.HIGH)
        time.sleep(0.25)
        # read and print second
        if (GPIO.input(self.INPUT_PIN) == GPIO.LOW): # Physically read the pin now
            return False
        else:
            return True
        
    #Not actually controlling anything, simulate door open with buzzer
    def open_door(self):
        self.pwm.start(50)
        time.sleep(2)
        self.pwm.stop()
    
    def clear_selections(self):
        self.outgoing = [False, False, False, False, False]
        self.incoming = [False, False, False, False, False]

    
    #Save reading from tool slots for comparing later
    def snapshot_slots(self):
        self.snapshot = self.read_shelf()

    def clear_snapshot(self):
        self.snapshot = None

    def start_admin_mode(self, tag):
        self.admin_mode = True
        self.admin_id = tag.id
        self.admin_tagid = tag.tag_id
        self.user_in = None
        self.clear_selections()
        self.snapshot_slots()

    def end_admin_mode(self):
        self.admin_mode = False
        self.admin_tagid = None
        self.admin_id = None
        self.clear_snapshot()