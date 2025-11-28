import * as React from 'react';
import { FluentProvider, Input, Combobox, Option, Theme } from '@fluentui/react-components';
import { IInputs } from './generated/ManifestTypes';
import { DataService, IDataService } from './DataService';

export interface ITableSelectorControlProps {
  controlContext: ComponentFramework.Context<IInputs>;
  name?: string;
  isDisabled: boolean,
  theme?: Theme
  isCanvasApp?: boolean, 
  dataService: IDataService
}

const data = ['Eugenia', 'Bryan', 'Linda', 'Nancy', 'Lloyd', 'Alice', 'Julia', 'Albert'].map(
  item => ({ label: item, value: item })
);

// Static styles object since we can't use hooks in class components
const styles = {
  root: {
    width: "100%",
    minWidth: "200px"
  }
};

export class TableSelectorControl extends React.Component<ITableSelectorControlProps> {

  private name?: string;
  private isDisabled: boolean;
  private theme?: Theme;
  private isCanvasApp?: boolean;

  private dataService: IDataService

  constructor(props: ITableSelectorControlProps) {
    super(props);

    // Assign constructor parameters from props
    this.name = props.name;
    this.isDisabled = props.isDisabled;
    this.theme = props.theme;
    this.isCanvasApp = props.isCanvasApp;
    this.dataService = props.dataService;
  }

  componentDidMount(): void {
    console.log('Component mounted. Loading tables...');

    this.dataService.getAvailableTables()
      .then((tables) => {
        console.log(tables);

        this.setState({ tables: tables });

        return tables; 
        //Example of tables data:
        // [{"Name":"Account","SchemaName":"account"},
        // {"Name":"Contact","SchemaName":"contact"},{"Name":"Lead","SchemaName":"lead"}]
      })
      .catch((error) => {
        console.error('Error loading tables:', error);
      });
  }

  render(): React.ReactElement {
    const myTheme = this.isDisabled && this.isCanvasApp === false
      ? {
        ...this.theme,
        colorCompoundBrandStroke: this.theme?.colorNeutralStroke1,
        colorCompoundBrandStrokeHover: this.theme?.colorNeutralStroke1Hover,
        colorCompoundBrandStrokePressed: this.theme?.colorNeutralStroke1Pressed,
        colorCompoundBrandStrokeSelected: this.theme?.colorNeutralStroke1Selected,
      }
      : this.theme;

    return (
      <FluentProvider theme={myTheme} >

        {!this.isDisabled || this.isCanvasApp === true
          ? <Combobox
            //value={this.name}          
            appearance='filled-darker'
            style={styles.root}
            listbox={{ style: styles.root }}
            readOnly={this.isDisabled}
            disabled={this.isDisabled && this.isCanvasApp === true}
            placeholder="Select an option..."
            aria-label="Table selector"
          >
            {data.map((item) => (
              <Option
                key={item.value}
                value={item.value}
                text={item.label}
                style={styles.root}
              >
                <span style={styles.root}>{item.label}</span>
              </Option>
            ))}
          </Combobox>
          : <Input
            value={this.name}
            appearance='filled-darker'
            style={styles.root}
            readOnly={this.isDisabled}
          />
        }
      </FluentProvider>
    );
  }
}
