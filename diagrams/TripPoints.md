
## ** Pick Up and Drop Off Point Flow Diagram
-------- 

```mermaid

graph TD
subgraph Pick Up
    A[Stand By] --> B[Start Moving to Loading Location]
    B --> C[Start Loading]
    C --> R[Arrive to Loading Location]
    R --> D[Finish loading]
end
subgraph Drop Off
    F[Start Moving to of Loading Location] --> G[Arrived To Destination]
    G --> M[Start Off loading]
    M --> P[Finish Off Loading]
    P --> X[Completed and Missing Receiver Code] --> J
    X --> U[Delivered And Needs Confirmation]
    U --> J[Delivered]
end










