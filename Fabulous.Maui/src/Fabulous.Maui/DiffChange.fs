namespace Fabulous

type DiffChangeValue =
    | Create of value: obj
    | Update of newValue: obj
    | Clear
    
type DiffChange =
    { Key: string
      Value: DiffChangeValue }

