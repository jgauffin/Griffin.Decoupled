Griffin.Decoupled
=================

Decouple that application, will you?

## Introduction
http://blog.gauffin.org/2012/10/writing-decoupled-and-scalable-applications-2/

## Tutorials
http://blog.gauffin.org/2012/10/introducing-griffin-decoupled/

## Samples
https://github.com/jgauffin/Samples/tree/master/Griffin.Decoupled

## Features

All these features can be turned on/off.

* Sync/Async dispatching of commands/events
* Automatic retry of failing commands
* Store commands (commands will be executed at the next start if the app crashes)
* Hold domain events until a transaction successfully committs (events will be deleted if it fails).
