# pinkeen - the prototype DCP / Minnow client on .NET

## Introduction
This is a client for the Domain Chat Protocol, and a set of libraries for
writing your own custom clients.

The spec is actively being developed, as is this client/library, Pinkeen,
the native C++ client [Gilligan](https://github.com/DCP-Project/gilligan-prototype),
and the server [Minnow](https://github.com/DCP-Project/minnow-prototype).

## Requirements
* .NET Framework - currently only tested on 4.5.1, but targetting 2.0+.
* Currently development is occurring on Visual Studio 14 CTP 4, but development
  should be supported as far back as VS2005 if you really wanted to.

## Building
* ProtoLib is designed to be a portable, low-level protocol library that is
  suitable for any use and any runtime.
* StoreConnection is an implementation of DCPProject.ProtoLib.Connection for
  the WinRT socket API, using Windows.Networking.
* PlatformConnection is an implementation of DCPProject.ProtoLib.Connection
  for the traditional .NET socket API, using System.Net.
* Pinkeen is a WPF/XAML client/test application.
