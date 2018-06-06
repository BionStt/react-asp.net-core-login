import * as React from 'react';
import { Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store';
import * as UserStore from '../../store/User';
import './HomePage.css';
import { Header } from '../../components/common/Header';
import * as UserState from '../../store/User';


// At runtime, Redux will merge together...
type OwnProps = RouteComponentProps<{}>;
type StateToPropsType = {
    userState: UserState.UserState
}
type DispathToPropsType = {
    userStateActions: any
}
type HomePageProps = StateToPropsType & DispathToPropsType & OwnProps;
type DistributionPageState = {

}

class HomePage extends React.Component<HomePageProps, {}> {
    constructor(props: any) {
        super(props);      
    }
    componentWillMount() {
        this.props.userState.user.id === 0 ? this.props.userStateActions.logout() : null;
    }
    public render() {
        return <div>
            <Header 
                redirectHome={() => { this.props.history.push('/') }}                
            />                               
            <div><h2>{"HOMEPAGE"}</h2></div>
            <div>
                <img style={{ width:"100%" }} id="f3" src={require('../../assets/images/nature.jpg')} />
            </div> 
        </div>;
    }
}

const mapStateToProps = (state: ApplicationState, ownProps: OwnProps): StateToPropsType => ({
    userState: state.user    
});
const mapDispatchToProps = (dispatch: any): DispathToPropsType => {
    return {
        userStateActions: bindActionCreators(UserState.actionCreators, dispatch)          
    };
};
export default withRouter(
    connect<StateToPropsType, DispathToPropsType, OwnProps>(mapStateToProps, mapDispatchToProps)(HomePage)
);