import React from 'react'
import styles from './styles.scss'

import { Icon } from 'semantic-ui-react'

export default function GenericTool (props) {
  return (
    <div onClick={props.onClick} className={styles.tool}>
      <Icon name={props.name} size='large' className={styles.toolIcon} />
    </div>
  )
}
