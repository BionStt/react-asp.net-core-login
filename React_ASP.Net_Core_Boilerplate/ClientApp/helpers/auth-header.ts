import { User } from '../commonTSDefinitions/user';
export function authHeader(isContentType) {
    // return authorization header with jwt token
    let jsonUser: string | null = process.env.IS_BROWSER ? sessionStorage.getItem('user') : null;
    let user: User = jsonUser ? JSON.parse(jsonUser) : new User();     
    if (user && user.token) {
        let header = isContentType ? { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + user.token } : { 'Authorization': 'Bearer ' + user.token }
        return header;
    } else {
        return { 'Content-Type': 'application/json' };
    }
}