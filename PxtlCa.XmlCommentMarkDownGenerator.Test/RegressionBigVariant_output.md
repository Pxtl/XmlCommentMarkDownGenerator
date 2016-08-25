# PxtlCa.BigVariant.Core #

## T:BigVariant

 The SQLCLR BigVariant type 

---
#### P:BigVariant.Type

 The SQL CLR Type of the BigVariant, as a String accessible from SQL. See https://msdn.microsoft.com/en-us/library/ms131092.aspx for information about the types. 

##### Example: 

###### SQL code

```
    DECLARE @testInput bit = 1
    DECLARE @testVar BigVariant = dbo.BigVariantFromVariant(@testInput)
    SELECT @testVar.Type -- returns 'SqlBoolean'
```

---
#### P:BigVariant.AsVariant

 If the BigVariant contains a SQL_VARIANT-compatible type, get its contents. Will throw an exception if the type is not SQL_VARIANT-compatible. 

>DATETIME2s will be converted to DateTimes as interrim, use AsDateTime2 if you need DATETIME2s

##### Example: 

###### SQL code

```
    DECLARE @testInput float = 1.79E+308
    DECLARE @testVar BigVariant = dbo.BigVariantFromVariant(@testInput)
    SELECT @testVar.AsVariant
    -- returns 1.79E+308
```

---
#### P:BigVariant.AsDateTime2

 If the BigVariant contains a DATETIME2 (DateTime in CLR), get its contents. Will throw an exception if the type is not DATETIME2. 

##### Example:  DATETIME2 Unit Test 

###### SQL code

```
    DECLARE @testInput DateTime2 = convert(DateTime2, '0001-01-01 11:59:00 PM')
    DECLARE @testVar BigVariant = dbo.BigVariantFromDateTime2(@testInput)
    SELECT 'success' WHERE @testInput = @testVar.AsDateTime2
```

---
#### P:BigVariant.AsXml

 If the BigVariant contains XML, get its contents. Will throw an exception if the type is not XML. 

##### Example:  Use xpath to pull data out of an Xml BigVariant. 

###### SQL code

```
    DECLARE @testInput Xml = convert(Xml
        , '<?xml version="1.0"?><catalog>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    	+ '<book id="bk101"><author>Gambardella, Matthew</author><title>XML Developer''s Guide</title><genre>Computer</genre><price>44.95</price><publish_date>2000-10-01</publish_date><description>An in-depth look at creating applications with XML.</description></book>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    	+ '</catalog>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    )
    DECLARE @testVar BigVariant = dbo.BigVariantFromXml(@testInput)
    SELECT @testVar.AsXml.Query('/catalog/book/author/text()')
    -- returns 'Gambardella, Matthew' as XML
```

---
#### P:BigVariant.AsString

 If the BigVariant contains NVARCHAR(MAX) or similar long SqlString object, get its contents. Will throw an exception if the type is not NVARCHAR(MAX) or similar long SqlString object. 

##### Example:  Unit test. 

###### SQL code

```
    DECLARE @testString NVARCHAR(2000) = 'Silence is foo'
    DECLARE @testVar BigVariant = dbo.BigVariantFromVariant(@testString)
    SELECT 'success' WHERE @testString = @testVar.AsString
```

---
#### P:BigVariant.AsBinary

 If the BigVariant contains VARBINARY(MAX) or similar long SqlBinary object, get its contents. Will throw an exception if the type is not VARBINARY(MAX) or similar long SqlBinary object. 

---
#### M:BigVariant.Read(System.IO.BinaryReader)

 Implement IBinarySerialize.Read because SQL stores everything as binary even temporarily. Internal plumbing method, don't use. 

---
#### M:BigVariant.Write(System.IO.BinaryWriter)

 Implement IBinarySerialize.Write because SQL stores everything as binary even temporarily. Internal plumbing method, don't use. 

---
## T:UserDefinedFunctions

 This class collects together various SQL User-Defined-functions used to construct BigVariant values out of various SQL types. 

---
#### M:UserDefinedFunctions.BigVariantFromXml(System.Data.SqlTypes.SqlXml)

 Take the given XML typed SQL object and convert it into a BigVariant. 

|Name | Description |
|-----|------|
|value: |an XML object to wrap in a BigVariant|
**Returns**: A BigVariant containing the given XML object

##### Example: 

###### SQL code

```
    DECLARE @testInput Xml = convert(Xml
        , '<?xml version="1.0"?><catalog>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    	+ '<book id="bk101"><author>Gambardella, Matthew</author><title>XML Developer''s Guide</title><genre>Computer</genre><price>44.95</price><publish_date>2000-10-01</publish_date><description>An in-depth look at creating applications with XML.</description></book>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    	+ '</catalog>' + CHAR(13)+CHAR(10) + CHAR(13)+CHAR(10)
    )
    DECLARE @testVar BigVariant = dbo.BigVariantFromXml(@testInput)
    SELECT 'success' WHERE CONVERT(NVARCHAR(4000), @testInput) = CONVERT(NVARCHAR(4000), @testVar.AsXml)
```

---
#### M:UserDefinedFunctions.BigVariantFromVariant(System.Object)

 Take the given SQL_VARIANT object and convert it into a BigVariant. 

|Name | Description |
|-----|------|
|value: |a SQL_VARIANT to wrap in a BigVariant|
**Returns**: A BigVariant containing the given SQL_VARIANT

>DateTime2s will be converted to DateTimes as intermediate, use BigVariantFromDateTime2 for proper datetime2.

##### Example: 

###### SQL code

```
    DECLARE @testInput float = 1.79E+308
    DECLARE @testVar BigVariant = dbo.BigVariantFromVariant(@testInput)
    SELECT @testVar.AsVariant
    -- returns 1.79E+308
```

---
#### M:UserDefinedFunctions.BigVariantFromDateTime2(System.Nullable{System.DateTime})

 Take the given DATETIME2 object and convert it into a BigVariant. 

|Name | Description |
|-----|------|
|value: |a DATETIME2 to wrap in a BigVariant|
**Returns**: A BigVariant containing the given DATETIME2

---
#### M:UserDefinedFunctions.BigVariantFromString(System.Data.SqlTypes.SqlString)

 Take the given NVARCHAR(MAX) or TEXT or NTEXT object and convert it into a BigVariant. 

|Name | Description |
|-----|------|
|value: |an NVARCHAR(MAX) or TEXT or NTEXT to wrap in a BigVariant|
**Returns**: A BigVariant containing the given NVARCHAR(MAX) or TEXT or NTEXT

##### Example: 

###### SQL code

```
    DECLARE @testString NVARCHAR(MAX)
    DECLARE @testVar BigVariant
    SET @testString = 'Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus. Vivamus elementum semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10)
     + 'Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel, luctus pulvinar, hendrerit id, lorem. Maecenas nec odio et ante tincidunt tempus. Donec vitae sapien ut libero venenatis faucibus. Nullam quis ante. Etiam sit amet orci eget eros faucibus tincidunt. Duis leo. Sed fringilla mauris sit amet nibh. Donec sodales sagittis magna. Sed consequat, leo eget bibendum sodales, augue velit cursus nunc, quis gravida magna mi a libero. Fusce vulputate eleifend sapien. Vestibulum purus quam, scelerisque ut, mollis sed, nonummy id, metus. Nullam accumsan lorem in dui. Cras ultricies mi eu turpis hendrerit fringilla. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; In ac dui quis mi consectetuer lacinia. Nam pretium turpis et arcu. Duis arcu tortor, suscipit eget, imperdiet nec, imperdiet iaculis, ipsum.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10)
     + 'Sed aliquam ultrices mauris. Integer ante arcu, accumsan a, consectetuer eget, posuere ut, mauris. Praesent adipiscing. Phasellus ullamcorper ipsum rutrum nunc. Nunc nonummy metus. Vestibulum volutpat pretium libero. Cras id dui. Aenean ut eros et nisl sagittis vestibulum. Nullam nulla eros, ultricies sit amet, nonummy id, imperdiet feugiat, pede. Sed lectus. Donec mollis hendrerit risus. Phasellus nec sem in justo pellentesque facilisis. Etiam imperdiet imperdiet orci. Nunc nec neque. Phasellus leo dolor, tempus non, auctor et, hendrerit quis, nisi. Curabitur ligula sapien, tincidunt non, euismod vitae, posuere imperdiet, leo. Maecenas malesuada. Praesent congue erat at massa. Sed cursus turpis vitae tortor. Donec posuere vulputate arcu. Phasellus accumsan cursus velit.' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10)   
    SET @testVar = dbo.BigVariantFromString(@testString)
```

---
#### M:UserDefinedFunctions.BigVariantFromBinary(System.Data.SqlTypes.SqlBinary)

 Take the given VARBINARY(MAX) or IMAGE or other binary object and convert it into a BigVariant. 

|Name | Description |
|-----|------|
|value: |an VARBINARY(MAX) or IMAGE or other binary to wrap in a BigVariant|
**Returns**: A BigVariant containing the given VARBINARY(MAX) or IMAGE or other binary

---

