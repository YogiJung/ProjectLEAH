package ExampleTCPServer.TCPChannel;

import ExampleTCPServer.TCPChannel.Handlers.TCPInboundHandler;
import ExampleTCPServer.TCPChannel.Handlers.TCPOutboundHandler;
import io.netty.channel.Channel;
import io.netty.channel.ChannelInitializer;
import io.netty.channel.ChannelPipeline;

public class ExampleTCPChannelInitializer extends ChannelInitializer {

    TCPInboundHandler tcpInboundHandler = new TCPInboundHandler();
    TCPOutboundHandler tcpOutboundHandler = new TCPOutboundHandler();

    @Override
    protected void initChannel(Channel channel) throws Exception {
        ChannelPipeline cp = channel.pipeline();
        cp.addFirst("TCPInboundH", tcpInboundHandler);
        cp.addAfter("TCPInboundH", "TCPOutboundH", tcpOutboundHandler);
    }
}
