import { UserRecord } from './UserRecord';
import { UserToken } from './UserToken';

export interface UserResponse {
    user : UserRecord;
    userToken: UserToken;
}

