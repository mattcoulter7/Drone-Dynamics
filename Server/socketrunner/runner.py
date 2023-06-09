from typing import Callable
from twisted.internet import reactor
from twisted.internet.address import IAddress

from .messaging.packet import Packet
from .messaging.messenger import Messenger


class SocketRunner():
    def __init__(
            self,
            *ends):
        self.ends = ends
        self._ends = {
            end.protocol: end for end in ends
        }
        self._default_protocol = self.ends[0].protocol if self.ends else None

    def start(self):
        """Start the server // Connect to a server"""
        for end in self.ends:
            end.start()

        reactor.run()

    def send(
            self,
            data: bytes,
            protocol: str = None,
            addr: IAddress = None):
        """Send a message for a given method"""
        protocol = protocol or self._default_protocol
        end = self._ends.get(protocol)
        if end is None:
            raise Exception(f"Invalid Method {protocol}")

        end.send(data, addr)

    def broadcast(
            self,
            data: bytes,
            protocol: str = None):
        """Send a message for a given method"""
        protocol = protocol or self._default_protocol
        end = self._ends.get(protocol)
        if end is None:
            raise Exception(f"Invalid Method {protocol}")

        end.broadcast(data)


class ManagedRunner(SocketRunner):
    """Handles structured data instead of raw bytes. Utilises listeners"""
    def __init__(self, *ends):
        super().__init__(*ends)
        for end in ends:
            end.receive_callback = self._receive

        self.messenger = Messenger()

    def send(self, data: Packet, protocol: str = None, addr: IAddress = None) -> None:
        return super().send(data.as_bytes(), protocol, addr)

    def broadcast(self, data: Packet, protocol: str = None) -> None:
        return super().broadcast(data.as_bytes(), protocol)

    def on(self, event: str, callback: Callable):
        self.messenger.sub(event, callback)

    def _receive(self,  data: bytes, addr: IAddress = None) -> Packet:
        packet = Packet.from_bytes(data)
        if packet is None:
            return
        
        self.messenger.pub(
            packet.event,
            packet,
            addr
        )
