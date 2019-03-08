# RawParameter

### 改动，　新增stage字段

## type 



```typescript
interface RawParameter {
   stepId:string; //不可改
   routeId:string; //不可改
   stage:number; //默认为0
   code:string;  //目前有　PARAM，JS；　默认PARAM
   payload:string;
}
```
