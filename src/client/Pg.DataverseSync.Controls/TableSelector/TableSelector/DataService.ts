
export interface IDataService {
    getAvailableTables(): Promise<ITableInfo[]>;
}

export interface ITableInfo {
    Name: string;
    SchemaName: string;
}

export class DataService implements IDataService {

    private webApi: ComponentFramework.WebApi;

    constructor(webApi: ComponentFramework.WebApi) {
        this.webApi = webApi;
    }

    async getAvailableTables(): Promise<ITableInfo[]> {

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
        try {
            var result = await anyWebAPI.execute(execute_pg_getunsynchronizedtables_Request);
            if (result.ok) {
                console.log("Custom API executed successfully.");
                var jsonResponse = await result.json();
                if (jsonResponse) {
                    try {
                        console.log("Extracting data from response body.");
                        var tables: ITableInfo[] = jsonResponse["tables"];
                        console.log(`${tables.length} tables retrieved.`);
                        return tables;
                    }
                    catch (error) {
                        console.log("Error parsing response body.");
                        throw new Error("Failed to parse response body");
                    }
                }
                else {
                    console.log("Cannot extract data from response body");
                    throw new Error("Cannot extract data from response body");
                }
            }
            else {
                console.log("Error executing custom API.");
                console.log(result);
                throw new Error(`Custom API execution failed with status: ${result.status}`);
            }
        } catch (error) {
            console.error("Unhandled error in getAvailableTables:", error);
            return [];
        }
    }
}