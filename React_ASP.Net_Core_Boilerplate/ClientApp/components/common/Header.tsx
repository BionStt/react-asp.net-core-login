import * as React from 'react';
import * as Endpoints from '../../consts/endpoints';


type HeaderProps = {    
    redirectHome: Function;
}

export class Header extends React.Component<HeaderProps, any> {
    render() {
        return (
            <div style={{ paddingTop: "50px" }}>
                <h1>PUT YOUR HEADER HERE</h1>
            </div>
        );
    }
}