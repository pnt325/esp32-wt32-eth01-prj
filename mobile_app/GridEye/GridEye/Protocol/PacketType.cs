using System;
using System.Collections.Generic;
using System.Text;

namespace GridEye.Protocol
{
    public enum PacketType
    {
        Data,               // send data
        Ack,                // reponse send data success 
        Nack,               // reponse send data failure/ task handle failure
        Ctrl,               // control message without reponse
        Unknown             // unknow type packet
    }
}
