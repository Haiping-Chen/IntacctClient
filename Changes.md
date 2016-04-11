# Change log

## 0.4.2
 - added basic journal support (thanks to David Luxford for contribution!)

## 0.4.1
 - extracted `IIntacctClient` interface to facilitate testing

## 0.4.0
 - enabled operations to return empty (null) results, where appropriate
 - public API surface cleanup

## 0.3.5
 - added ability to retrieve entities via the `GetEntityOperation<TEntity>` class

## 0.3.2
 - added convenience contructor for IntacctDate that takes a DateTime value

## 0.3.1
 - added "create invoice" operation

## 0.3.0
 - added constructors with required fields for Intacct entities

## 0.2.2
 - improved handling of authentication failures when establishing API session

## 0.2.1
 - throw exception when receiving unexpected response while attempting to establish API session

## 0.2.0
 - introduced IIntacctServiceResponse interface to facilitate testing

## 0.1.0
 - introduced IIntacctSession interface to facilitate testing

## 0.0.3
 - made methods on IntacctClient virtual to simplify testing

## 0.0.2
 - removed support for UWP10 due to conflict with Azure builds

## 0.0.1
 - initial release, support for creating customer record