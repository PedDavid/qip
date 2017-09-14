import React from 'react'
import styles from './styles.scss'

import { Icon } from 'semantic-ui-react'

export default function GenericTool (props) {
  return (
    <div className={styles.tool}>
      <Icon onClick={props.onClick} name={props.name} size='large' className={styles.toolIcon} />
    </div>
  )
}
