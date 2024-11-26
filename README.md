# RedyLang Compiler
Redylang is a **strongly typed**, **compiled**, **expression-based** programming language and will be **garbage collected** in the future.

## Main implemented features
* Type System
  * Structs
  * Traits ( interfaces )
  * Methods on any types
* Functions
* Basic arithmetic and comparisons
* Expressions like if-else, blocks, return, etc.
* Explicit returns
* Variables
* Module/Import system

## Target output
Redylang compiles down to my custom bytecode called rasm. You can learn more about it here: https://github.com/RedyG/rasm_vm

## Exemple
```rust
mod test;

type Square = struct {
  size: i32
};

impl Square {
  fn new(size: i32) -> Square {
    return Square { size: size };
  }

  fn area(self) -> i32 {
    return self.size * self.size;
  }
}

fn main() {
  var square = Square::new(10);

  square.area() + fib(10); // 155
}

fn fib(n: i32) -> i32 {
  return if n < 2 {
    n
  } else {
    fib(n - 1) + fib(n - 2)
  };
}

```

## What's next?
* GC when the VM supports it.
* Generics
* A standard library
* Escape analysis
* Pattern matching
* Move semantics
* Iterators
* More control flow
* Much, much more
