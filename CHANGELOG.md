# Changelog
All notable changes to this project will be documented in this file.

## [1.6.0] - 2018-09-07
### Added
- Create interface IPacketHandlerRepository. This can be passed to RadiusServer to implement a completely customised packet handler repository, for example from a database. Packet handler methods on RadiusServer will be removed in the future.


## [1.5.0] - 2018-07-27
### Added
- Allow mapping packet handlers to IPAddress.Any and IPNetwork ranges. Using IPAddress.Any is not recommended in normal use.


## [1.4.0] - 2018-05-19
### Changes
- Use IRadiusDictionary interface for custom radiusdictionary implementations
