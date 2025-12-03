import { Theme } from "@fluentui/react-theme";
import { IInputs, IOutputs } from "./generated/ManifestTypes";
import { TableSelectorControl, ITableSelectorControlProps } from "./TableSelectorControl";
import * as React from "react";
import { DataService, DataServiceMock } from "./DataService";

export class TableSelector implements ComponentFramework.ReactControl<IInputs, IOutputs> {
    private notifyOutputChanged: () => void;
    private tableSchemaName: string | undefined;

    /**
     * Empty constructor.
     */
    constructor() {
        // Empty
    }

    /**
     * Used to initialize the control instance. Controls can kick off remote server calls and other initialization actions here.
     * Data-set values are not initialized here, use updateView.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to property names defined in the manifest, as well as utility functions.
     * @param notifyOutputChanged A callback method to alert the framework that the control has new outputs ready to be retrieved asynchronously.
     * @param state A piece of data that persists in one session for a single user. Can be set at any point in a controls life cycle by calling 'setControlState' in the Mode interface.
     */
    public init(
        context: ComponentFramework.Context<IInputs>,
        notifyOutputChanged: () => void,
        state: ComponentFramework.Dictionary
    ): void {
        this.notifyOutputChanged = notifyOutputChanged;
    }

    /**
     * Called when any value in the property bag has changed. This includes field values, data-sets, global values such as container height and width, offline status, control metadata values such as label, visible, etc.
     * @param context The entire property bag available to control via Context Object; It contains values as set up by the customizer mapped to names defined in the manifest, as well as utility functions
     * @returns ReactElement root react element for the control
     */
    public updateView(context: ComponentFramework.Context<IInputs>): React.ReactElement {

        this.tableSchemaName = context?.parameters?.tableName?.raw ?? "";

        const dataService = this.isRunningOnLocalhost(context) 
            ? new DataServiceMock() 
            : new DataService(context.webAPI);

        const props: ITableSelectorControlProps = {
            controlContext: context,
            tableSchemaName: this.tableSchemaName,
            isDisabled: false,
            theme: context?.fluentDesignLanguage?.tokenTheme as Theme,
            isCanvasApp: context?.parameters?.isCanvas?.raw === "Yes",
            dataService: dataService, 
            onChange: this.onChange.bind(this)
        };
        return React.createElement(
            TableSelectorControl, props
        );
    }

    /**
     * It is called by the framework prior to a control receiving new data.
     * @returns an object based on nomenclature defined in manifest, expecting object[s] for property marked as "bound" or "output"
     */
    public getOutputs(): IOutputs {
		return {
			tableName: this.tableSchemaName 
		  };
    }

    private onChange(value: string): void{
        this.tableSchemaName = value; 
        this.notifyOutputChanged(); 
    }

    public destroy(): void {
        // Add code to cleanup control if necessary
    }

    private isRunningOnLocalhost(context: ComponentFramework.Context<IInputs>): boolean {
        try {
            if (typeof window !== 'undefined' && window.location) {
                const hostname = window.location.hostname;
                if (hostname === 'localhost' || hostname === '127.0.0.1' || hostname.includes('localhost')) {
                    return true;
                }
            }
            return false;
        } catch (error) {
            console.log('Error detecting environment, defaulting to non-localhost mode:', error);
            return false;
        }
    }
}
