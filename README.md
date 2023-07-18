# ModbusRTU

A simple serial communication library of ModbusRTU

CRC16：Cyclical Redundancy Check

Create:
```C#
var modbusRtu = new ModbusRtu(new ModbusRtuConfig
{
    PortName = "COM1",
    BaudRate=9600,
    Parity=Parity.None,
    DataBit=8,
    StopBits=StopBits.One
}) ;
```

custom：
```C#
//coils
modbusRtu.SendCommand(0x01,0x01,0001,ColiStatus.ON);
modbusRtu.SendCommandAsync(0x01, 0x01, 0001, ColiStatus.OFF);
//Register
modbusRtu.SendCommand(0x01, 0x01, 0001,666);
modbusRtu.SendCommand(0x01, 0x01, 0018,123);
```

readcoils:
```C#
byte[] result=await modbusRtu.ReadCilsAsync(0x01,0000,2);
//byte[] result= modbusRtu.ReadCils(0x01,0000,2);
var boolList= modbusRtu.BytesOfReadColisToBoolList(result);
```

![image](https://github.com/chuanchuanYY/HLC_ModbusRTU/assets/107403292/66fa8cba-3869-42ea-8bad-bb57c5629f8f)


![image](https://github.com/chuanchuanYY/HLC_ModbusRTU/assets/107403292/a73a6558-3ec8-41e4-bb37-d67da8e1c8be)

```C#
byte[] result=await modbusRtu.ReadDiscreteInputsAsync(0x01,0000,2);
```

```C#
 byte[] result=await modbusRtu.ReadHoldingRegistersAsync(0x01,0000,3);
 var boolList= modbusRtu.BytesOfReadKeepRegisterToIntArr(result);

//byte[] result=await modbusRtu.ReadInputRegistersAsync(0x01,0000,3);
//var boolList= modbusRtu.BytesOfReadKeepRegisterToIntArr(result);
```

![image](https://github.com/chuanchuanYY/HLC_ModbusRTU/assets/107403292/2c865ed4-e21f-4824-aa3a-c88a92beb2bf)


![image](https://github.com/chuanchuanYY/HLC_ModbusRTU/assets/107403292/99c42439-acf3-4ab8-9dbe-b3426a9efd74)


```C#
byte[] result=  modbusRtu.WriteSingleRegister(0x01,0000,6666);
```

```C#
 byte[] result=await  modbusRtu.WriteMultipleRegistersAsync(0x01,0000, 5, new int[] { 123, 456, 789, 666, 5 });
```

