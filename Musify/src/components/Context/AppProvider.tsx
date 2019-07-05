import React from "react";
import { MusicStoreContext } from "./Context";
import NetInfo from "@react-native-community/netinfo";
import { AsyncStorageUtis } from "../AsnyStorageUtils";
import { User } from "react-native-google-signin";

interface Props {
}

interface States {
    user?: any;
    network: any;
    serverConnection: any;
    unsubscribe: any;
}

export class AppProvider extends React.Component<Props, States> {
    constructor(props: any)
    {
        super(props);
        this.state = {
            user: null,
            network: null,
            serverConnection: null,
            unsubscribe: null
        }
    }

    componentDidMount() {
        if (this.state.user == null)
            AsyncStorageUtis.getItem("user").then((data) => {
                if (data != null)
                 this.setState({user: JSON.parse(data)})
            }, () => {})
        let me = this;
        NetInfo.fetch().then((state) => {
            let connected = state.type != 'none' && state.type != 'unknown';
            let serverConnection = me.state.serverConnection;
            if (!connected)
                serverConnection = false;
            me.setState({network: connected, serverConnection: serverConnection})
        });
        this.setState({unsubscribe: NetInfo.addEventListener(state => {
            me.handleConnectivityChange(state, me)
        })})
    }
    
    handleConnectivityChange(connectionInfo : any, me: any) {
        let connected = connectionInfo && connectionInfo.type != 'none' && connectionInfo.type != 'unknown';
        let serverConnection = me.state.serverConnection;
        if (!connected)
            serverConnection = false;
        me.setState({network: connected, serverConnection: serverConnection})
    }

    logout() {
        AsyncStorageUtis.removeItem("user").then(() => {
            this.setState({user: null})
        })
    }

    render() {
        let me : any = this;
        let data = {
            user: this.state.user,
            network: this.state.network,
            serverConnection: this.state.serverConnection,
            logout: () => {this.logout()},
            changeProfile: function (user: User) : Promise<any> {
                return new Promise<any>((resolve, reject) => {
                    if (me.state.user)
                    {
                        AsyncStorageUtis.setItem("user", JSON.stringify(user)).then(() => {
                            me.setState({user}, () => resolve())
                        })
                    }
                })
            },
            login: (user: any) => {this.setState({user: user})}
        };
        return (<MusicStoreContext.Provider value={data as any}>
                {this.props.children}
        </MusicStoreContext.Provider>)
    }

    componentWillUnmount()
    {
        this.state.unsubscribe();
    }
}