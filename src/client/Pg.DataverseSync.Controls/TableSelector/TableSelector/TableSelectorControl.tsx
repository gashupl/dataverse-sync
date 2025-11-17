import * as React from 'react';
import {FluentProvider, Input, Combobox, Option, Theme, makeStyles, tokens, mergeClasses} from '@fluentui/react-components';
 
export interface ITableSelectorControlProps {
  name?: string;
  isDisabled: boolean, 
  theme ?: Theme
  isCanvasApp?: boolean
}

const data = ['Eugenia', 'Bryan', 'Linda', 'Nancy', 'Lloyd', 'Alice', 'Julia', 'Albert'].map(
  item => ({ label: item, value: item })
);

const useStyles = makeStyles({
  root: {
    width: "100%",
    minWidth: "200px"
  }
})

export const TableSelectorControl: React.FC<ITableSelectorControlProps> = ({ name, isDisabled, theme, isCanvasApp }) => {
const styles = useStyles();
 
  const myTheme = isDisabled && isCanvasApp===false
    ? {...theme, 
      colorCompoundBrandStroke: theme?.colorNeutralStroke1,
      colorCompoundBrandStrokeHover: theme?.colorNeutralStroke1Hover,
      colorCompoundBrandStrokePressed: theme?.colorNeutralStroke1Pressed,
      colorCompoundBrandStrokeSelected: theme?.colorNeutralStroke1Selected,
    }
    : theme
  return (    
    <FluentProvider theme={myTheme} >

      {!isDisabled || isCanvasApp === true
        ? <Combobox
            //value={name}          
            appearance='filled-darker'
            className={styles.root}
            listbox={{ className: styles.root }}
            readOnly={isDisabled}
            disabled={isDisabled && isCanvasApp===true}
            placeholder="Select an option..."
            aria-label="Table selector"          
          >          
            {data.map((item) => (
              <Option 
                key={item.value} 
                value={item.value} 
                text={item.label}
                className={styles.root}
              >
                <span className={styles.root}>{item.label}</span>
              </Option>
            ))}
          </Combobox>
      : <Input
          value={name}          
          appearance='filled-darker'
          className={styles.root}
          readOnly={isDisabled}        
        />  
      }
    </FluentProvider>
    );
};
