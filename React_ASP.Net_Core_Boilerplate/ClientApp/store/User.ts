import { fetch, addTask } from 'domain-task';
import { Action, Reducer, ActionCreator } from 'redux';
import { AppThunkAction } from './';
import { User } from '../commonTSDefinitions/user';
import * as Endpoints from '../consts/endpoints';
import { apiCall } from '../helpers/api-helper';



// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface UserState {
    user: User;    
}
interface LogoutAction {
    type: 'LOGOUT';
}

type KnownAction = LogoutAction;

export const actionCreators = {
    logout: (): AppThunkAction<KnownAction> => (dispatch, getState) => {
        // Only load data if it's something we don't already have (and are not already loading)     
        let token = sessionStorage.getItem("user") ? JSON.parse(sessionStorage.getItem("user")).token : "";
        let fetchTask = apiCall(token.toString(), 'POST', Endpoints.logout, true, dispatch)
            .then(() => { 
                    window.location.pathname = "/login";
                }
            ).catch(()=>{
                    window.location.pathname = "/login";
                }
            );
        sessionStorage.removeItem('user'); 
        addTask(fetchTask); // Ensure server-side prerendering waits for this to complete
        dispatch({ type: 'LOGOUT' });
    }   
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.
export const reducer: Reducer<UserState> = (state: UserState, incomingAction: Action) => {
    const action = incomingAction as KnownAction;
    switch (action.type) {
        case 'LOGOUT':
            // Only accept the incoming data if it matches the most recent request. This ensures we correctly
            // handle out-of-order responses.
            return {
                ...state,
                user: new User()
            };
        
        default:            
            const exhaustiveCheck: any = action;
    }
    let unloadedState: UserState = { 
        user: new User(),        
    };
    let parsedUser = new User(); 
    if (process.env.IS_BROWSER && sessionStorage.getItem('user')) {       
        const user: string | null = sessionStorage.getItem('user');
        parsedUser = JSON.parse(user ? user : '');
        unloadedState.user = parsedUser;
    }
    return { ...unloadedState };
}
