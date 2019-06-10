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
                resolve(data.json())
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
                resolve(data.json())
            }, (error) => {
                reject(error)
            })
        })
    }
}