// @flow

export const INVALID_IDX = -1

/** @param {Array<T>} array needs to be sorted */
export function findNearest<T> (array: Array<T>, value: number, mapper: (T) => number) {
  let left = 0
  let right = array.length - 1

  // Check out of bounds
  if (right < 1) {
    return 0
  }
  if (value < mapper(array[left])) {
    return left
  }
  if (value > mapper(array[right])) {
    return right
  }

  // Go binary search nearest
  let smallerDistance: number = INVALID_IDX
  let idxToReturn = INVALID_IDX

  while (left <= right) {
    const middle = (left + right) >> 1
    const currentElement = array[middle]
    const currentValue = mapper(currentElement)

    if (currentValue === value) {
      return middle
    }

    const currentDistance = Math.abs(currentValue - value)
    if (smallerDistance === INVALID_IDX || currentDistance < smallerDistance) {
      smallerDistance = currentDistance
      idxToReturn = middle
    }

    if (value > currentValue) {
      left = middle + 1
    } else {
      right = middle - 1
    }
  }
  return idxToReturn
}
