import React from 'react';

global.Symbol = require('core-js/es6/symbol');
require('core-js/fn/symbol/iterator');

// collection fn polyfills
require('core-js/fn/map');
require('core-js/fn/set');
require('core-js/fn/array/find');

import { SplashScreen } from './src/components/SplashScreen';

interface Props {
    ref: any;
    context: any;
}

interface States {
}

export default class App extends React.Component<Props, States> {
    render = () => (<SplashScreen {... this.props.ref} />)
}