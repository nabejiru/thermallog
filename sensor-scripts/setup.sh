#!/bin/bash

sudo apt update
sudo apt upgrade
sudo apt install python3-pip
mkdir /home/pi/thermallog
mkdir /home/pi/thermallog/log

sudo pip3 install --upgrade setuptools

sudo pip3 install --upgrade adafruit-python-shell
wget https://raw.githubusercontent.com/adafruit/Raspberry-Pi-Installer-Scripts/master/raspi-blinka.py
sudo python3 raspi-blinka.py