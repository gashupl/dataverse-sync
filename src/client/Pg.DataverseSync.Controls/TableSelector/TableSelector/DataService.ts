
export interface IDataService {
    getAvailableTables(onSuccess: (records: ComponentFramework.WebApi.RetrieveMultipleResponse) => void, onError: (error: string) => void): void;
}

export class DataService implements IDataService {

    private webApi: ComponentFramework.WebApi; 
    //private context: ComponentFramework.Context<any>;

    constructor(webApi: ComponentFramework.WebApi) {
        this.webApi = webApi;
    }
//Check: https://danielbergsten.wordpress.com/2022/01/31/calling-a-custom-api-from-pcf-custom-control/
    getAvailableTables(
        onSuccess: (records: ComponentFramework.WebApi.RetrieveMultipleResponse) => void, 
        onError: (error: string) => void): void {

        let execute_pg_getunsynchronizedtables_Request = {
            getMetadata: function () {
                return {
                    boundParameter: null,
                    parameterTypes: {},
                    operationType: 1, operationName: "pg_getunsynchronizedtables"
                };
            }
        };

        const anyWebAPI = this.webApi as any; 

        // anyWebAPI.createRecord("account", { name: "Sample Account" }).then(
        //     function (success: any) {
        //         console.log("Account created with ID: " + success.id);
        //     },
        //     function (error: any) {
        //         console.log(error.message);
        //     }
        // );  
        anyWebAPI.execute(execute_pg_getunsynchronizedtables_Request).then(
	        function success(response : any) {
                console.log("Custom API executed successfully.");
                if (response.ok) { return response.json(); }
                }
            ).then(function (responseBody : any) {
                console.log("Extracting data from response body.");
                var result = responseBody;
                console.log(result);
                var tables = result["tables"]; // Edm.String
                console.log(tables);
            }).catch(function (error: any) {
                console.log(error.message);
            });
    }

}