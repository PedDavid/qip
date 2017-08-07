import React from 'react'

import { Icon } from 'semantic-ui-react'

export default function GenericTool (props) {
  const style = {paddingTop: '5px', width: '38px', height: '38px'}
  return <Icon onClick={props.onClick} name={props.name} size='large' style={style} />
}
