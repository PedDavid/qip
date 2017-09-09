import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Move from './../../model/tools/Move'

module.exports = [
  {type: 'pen',
    icon: 'pencil',
    content: [
      {value: 'black', color: 'black', size: 'large'},
      {value: 'green', color: 'green', size: 'large'},
      {value: 'blue', color: 'blue', size: 'large'},
      {value: 'red', color: 'red', size: 'large'},
      {value: 'yellow', color: 'yellow', size: 'large'},
      {value: 'pink', color: 'pink', size: 'large'},
      {value: 'grey', color: 'grey', size: 'large'}
    ],
    lastValue: null,
    toolType: Pen
  },
  {type: 'width',
    icon: 'selected radio',
    content: [
      {value: 5, color: 'black', size: 'mini'},
      {value: 10, color: 'black', size: 'tiny'},
      {value: 15, color: 'black', size: 'small'},
      {value: 20, color: 'black', size: 'large'}
    ]
  },
  {type: 'eraser',
    icon: 'eraser',
    content: [
      {value: 5, color: 'black', size: 'tiny', type: 'normal'},
      {value: 10, color: 'black', size: 'small', type: 'normal'},
      {value: 15, color: 'black', size: 'large', type: 'normal'},
      {value: 15, color: 'black', size: 'large', type: 'stroke'}
    ],
    lastValue: null,
    toolType: Eraser
  },
  {type: 'move',
    icon: 'move',
    content: [],
    toolType: Move
  }
]
