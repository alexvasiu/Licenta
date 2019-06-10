export interface User {
    id: number,
    username: string,
    email: string,
    token: string,
    refreshToken: string,
    facebookId: string,
    googleId: string,
    userType: number
}

export interface UserForRegister {
    username: string,
    email: string,
    password: string,
    facebookId: string,
    googleId: string,
}