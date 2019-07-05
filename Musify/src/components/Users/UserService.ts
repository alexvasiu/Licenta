import { User, UserForRegister } from "./User";
import {BASE_URL} from './../AppResource';

export class UserService {
    public static register(user: UserForRegister): Promise<User> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "register", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(user)
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
        })
    }

    private static URL: string = BASE_URL + "user/";

    public static login(username: string, password: string) : Promise<User> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "login", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                  },
                body: JSON.stringify({
                    username: username,
                    password: password,
                }),
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
            .catch(() => {
                reject()
            })
        })
    }

    public static changePassword(changePasswordObj: any, token: string) : Promise<any> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "changePassword", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                  },
                body: JSON.stringify(changePasswordObj),
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
            .catch((error) => {
                reject(error)
            })
        })
    }

    public static changeProfile(changeProfileObj: any, token: string) : Promise<any> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "changeProfile", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json',
                    Authorization: 'Bearer ' + token
                  },
                body: JSON.stringify(changeProfileObj),
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
            .catch((error) => {
                reject(error)
            })
        })
    }

    public static loginFacebook(loginFb: any) : Promise<any> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "loginFacebook", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json'
                  },
                body: JSON.stringify(loginFb),
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
            .catch((error) => {
                reject(error)
            })
        })
    }

    public static loginGoogle(loginGoogle: any) : Promise<any> {
        return new Promise((resolve, reject) => {
            fetch(this.URL + "loginGoogle", {
                method: 'POST',
                headers: {
                    Accept: 'application/json',
                    'Content-Type': 'application/json'
                  },
                body: JSON.stringify(loginGoogle),
            }).then((data: any) => {
                if (data.ok)
                    resolve(data.json())
                else
                    reject()
            }, (error) => {
                reject(error)
            })
            .catch((error) => {
                reject(error)
            })
        })
    }
}