import React from "react";
import { AsyncStorage, NetInfo } from "react-native"
import { MusicStoreContext } from "./Context";

interface Props {
}

interface States {
    user?: any;
    network: any;
    serverConnection: any;
}

export class AppProvider extends React.Component<Props, States> {
    constructor(props: any)
    {
        super(props);
        this.state = {
            user: null,
            network: null,
            serverConnection: null
        }
    }

    componentDidMount() {
        if (this.state.user == null)
            AsyncStorage.getItem("user").then((data) => {
                if (data != null)
                    this.setState({user: JSON.parse(data)})
            },
            () => {})
        let me = this;
        NetInfo.getConnectionInfo().then((connectionInfo) => {
            let connected = connectionInfo.type != 'none' && connectionInfo.type != 'unknown';
            let serverConnection = me.state.serverConnection;
            if (!connected)
                serverConnection = false;
            me.setState({network: connected, serverConnection: serverConnection})
        });
        NetInfo.addEventListener(
            'connectionChange',
            (connectionInfo : any) => me.handleConnectivityChange(connectionInfo, me)
          );
    }
    
    handleConnectivityChange(connectionInfo : any, me: any) {
        let connected = connectionInfo && connectionInfo.type != 'none' && connectionInfo.type != 'unknown';
        let serverConnection = me.state.serverConnection;
        if (!connected)
            serverConnection = false;
        me.setState({network: connected, serverConnection: serverConnection})
    }

    logout() {
        this.setState({user: null})
        AsyncStorage.removeItem("user").then(() => {
        })
    }

    render() {
        let data = {
            user: this.state.user,
            network: this.state.network,
            serverConnection: this.state.serverConnection,
            logout: () => {this.logout()},
            login: (user: any) => {this.setState({user: user})}
        };
        return (<MusicStoreContext.Provider value={data as any}>
                {this.props.children}
        </MusicStoreContext.Provider>)
    }
}