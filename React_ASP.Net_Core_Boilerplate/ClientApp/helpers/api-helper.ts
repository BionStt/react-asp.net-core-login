import { authHeader } from './auth-header';


export function apiCall(data: any, type: string, endpoint: string, isContentType: boolean, dispatch:any) {
    
    return fetch(endpoint, {
        method: type,
        headers: authHeader(isContentType),
        body: isContentType ? (data ? JSON.stringify(data) : {}) : (data)
    }).then((response) => {        
            return response;
        });
}