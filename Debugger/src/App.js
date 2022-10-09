import "./App.css";
import axios from "axios";
import { useEffect, useState } from "react";
import "./index.css";

const initialChip8State = {
  registers: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
  i: 0,
  pc: 500,
  display: "",
  memory: [],
  stack: [],
  instructions: [],
};

const Memory = ({ memory, pc }) => {
  const rowSize = 32;
  const rows = [];
  let currentRow = [];

  for (let i = 1; i <= memory.length; i++) {
    currentRow.push({
      location: i - 1,
      hex: memory[i - 1],
      decimal: parseInt(memory[i - 1], 16),
    });

    if (i % rowSize === 0) {
      rows.push(currentRow);
      currentRow = [];
    }
  }

  return (
    <table>
      <tbody>
        {rows.map((row) => {
          return (
            <>
              <tr key={`memory-row-${row}`}>
                {row.map((cell) => {
                  const cellClass =
                    cell.location === pc || cell.location === pc + 1
                      ? "currentMemoryLocation"
                      : "";

                  const title = `location: ${cell.location}, decimal: ${cell.decimal}, hex: ${cell.hex}`;
                  return (
                    <td
                      key={`memory-cell-${cell.location}`}
                      className={cellClass}
                      title={title}
                    >
                      {cell.hex}
                    </td>
                  );
                })}
              </tr>
            </>
          );
        })}
      </tbody>
    </table>
  );
};

const App = () => {
  const [chip8State, setChip8State] = useState(initialChip8State);
  const [isExecuting, setIsExecuting] = useState(false);
  const [executionSpeed, setExecutionSpeed] = useState(10);
  const [loadedRom, setLoadedRom] = useState()

  useEffect(() => {
    let interval = {};

    if (isExecuting) {
      interval = setInterval(() => {
        step().then(() => console.log("Updated state."));
      }, executionSpeed);
    } else {
      clearInterval(interval);
    }

    return () => clearInterval(interval);
  }, [isExecuting, executionSpeed]);

  const loadRom = async (file) => {
    setLoadedRom(file)

    const formData = new FormData();

    formData.append("rom", file);

    const response = await axios.post(
      "https://localhost:7222/Chip8/load",
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );

    if (response.status !== 200) {
      alert("Upload error.");
      return;
    }

    setIsExecuting(false);
    setChip8State(initialChip8State);
    await getChip8State();
  };

  const reset = async () => {
    await loadRom(loadedRom)
  }

  const getChip8State = async () => {
    const { data } = await axios.get("https://localhost:7222/Chip8");

    setChip8State(data);
  };

  const step = async () => {
    const { data } = await axios.get("https://localhost:7222/Chip8/step");

    setChip8State(data);
  };

  const formattedDisplay = () => {
    return chip8State.display.replaceAll(0, "░").replaceAll(1, "█");
  };

  const startExecution = async () => {
    setIsExecuting(true);
  };

  const stopExecution = () => {
    setIsExecuting(false);
  };

  return (
    <div className="container">
      <h1>Debugger</h1>
      <input
        type="file"
        accept=".ch8"
        onChange={(e) => loadRom(e.target.files[0])}
      />
      <button onClick={startExecution} disabled={isExecuting}>
        Start
      </button>
      <button onClick={stopExecution} disabled={!isExecuting}>
        Stop
      </button>
      <button onClick={step} disabled={isExecuting}>
        Step
      </button>
      <button onClick={reset}>
        Reset
      </button>
      <label htmlFor="execution-speed">Execution speed:</label>
      <input
        name="execution-speed"
        type="number"
        value={executionSpeed}
        onChange={(e) => setExecutionSpeed(e.target.value)}
      />
      <div className="columns">
        <div className="column">
          <h2>Registers</h2>
          {chip8State.registers.map((_, index) => {
            return (
              <div className="row" key={`register-${index}`}>
                <span className="key">
                  V{index.toString(16).toUpperCase()}:
                </span>
                <span>{chip8State.registers[index]}</span>
              </div>
            );
          })}
        </div>
        <div className="column pcIStack">
          <h2>PC / I / Stack</h2>
          <div className="row">
            <span className="key">PC:</span>
            <span>{chip8State.pc}</span>
          </div>
          <div className="row">
            <span className="key">I:</span>
            <span>{chip8State.i}</span>
          </div>
          <div className="row">
            <span className="key">Stack:</span>
            <span>{chip8State.stack}</span>
          </div>
        </div>
        <div className="column">
          <h2>Instructions</h2>
          <div className="instructions">
            {chip8State.instructions.map((instruction, index) => {
              const className = chip8State.instructions[index].includes("NOOP")
                ? "instruction instruction--no-op"
                : "instruction";
              return (
                <div className={className} key={`instruction-${index}`}>
                  {chip8State.instructions[index]}
                </div>
              );
            })}
          </div>
        </div>
        <div className="column">
          <h2>Display</h2>
          <pre>{formattedDisplay(chip8State.display)}</pre>
        </div>
      </div>
      <div>
        <h2>Memory</h2>
        {<Memory memory={chip8State.memory} pc={chip8State.pc} />}
      </div>
    </div>
  );
};

export default App;
