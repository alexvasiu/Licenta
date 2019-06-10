import AsyncStorage from '@react-native-community/async-storage';

export class AsyncStorageUtis {

    static async getItem(key: string) : Promise<any> {
        let error : any = null, value : any = null;
        try {
            value = await AsyncStorage.getItem(key);
          } catch(e) {
            error = e;
          }
          return new Promise((resolve, reject) => {
              if (error != null)
                reject(error)
            resolve(value)
          });
    }

    static async removeItem(key: string) : Promise<any> {
        let error : any = null;
        try {
            await AsyncStorage.removeItem(key)
          } catch(e) {
            error = e;
          }
          return new Promise((resolve, reject) => {
              if (error != null)
                reject(error)
            resolve()
          });
    }

    static async setItem(key: string, value: string) : Promise<any> {
        let error : any = null;
        try {
            await AsyncStorage.setItem(key, value)
          } catch (e) {
            error(e)
          }
          return new Promise((resolve, reject) => {
              if (error != null)
                reject(error)
            resolve()
          });
    }
}