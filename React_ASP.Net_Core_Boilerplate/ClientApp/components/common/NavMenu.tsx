import * as React from 'react';
import { NavLink, Link, RouteComponentProps, withRouter } from 'react-router-dom';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import * as UserState from '../../store/User';
import { ApplicationState } from '../../store';
import $ from 'jquery';


type OwnProps = RouteComponentProps<{}>;
type StateToPropsType = {
    userState: UserState.UserState
}
type DispathToPropsType = {
    userStateActions: any    
}
type NavMenuProps = StateToPropsType & DispathToPropsType & OwnProps;

class NavMenu extends React.Component<NavMenuProps, {}> {

    collapseNavbar = () => {
        $('.collapse').collapse("hide");
    } 

    logout=()=>{
        this.props.userStateActions.logout();
    }

    public render()
    {
        return <div className='main-nav'>
                <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={'/'}>Menu</Link>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li onClick={()=>this.collapseNavbar()}>
                            <NavLink exact to={'/'} activeClassName='active'>
                                <span className='glyphicon glyphicon-home'></span> {"HOME"}
                            </NavLink>
                        </li>  
                    </ul>
                    <div onClick={() => this.logout()} className="menu_logout">
                        <span className='logout-icon glyphicon glyphicon-log-out'></span>
                        {"LOGOUT"}
                    </div>            
                </div>
            </div>
        </div>
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
    connect<StateToPropsType, DispathToPropsType, OwnProps>(mapStateToProps, mapDispatchToProps)(NavMenu)
);
