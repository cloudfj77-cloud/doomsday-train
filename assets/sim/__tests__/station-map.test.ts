import {it,expect} from 'vitest';
import {canWalk,stationBuildings,TRAIN_EXIT,roofHidden} from '../../shared/station-map.ts';

it('every seeded room and both return lanes are reachable from the train',()=>{
  for(const seed of [0,1,2]) {
    const queue=[[TRAIN_EXIT.x,TRAIN_EXIT.z]];
    const visited=new Set([queue[0]!.join(',')]);
    for(let i=0;i<queue.length;i++) {
      const [x,z]=queue[i]!;
      for(const [dx,dz] of [[0.5,0],[-0.5,0],[0,0.5],[0,-0.5]]) {
        const nx=x!+dx!,nz=z!+dz!,key=nx+','+nz;
        if(!visited.has(key)&&canWalk(seed,nx,nz)) {visited.add(key);queue.push([nx,nz]);}
      }
    }
    for(const b of stationBuildings(seed)) expect(visited.has(b.x+','+(b.z+1))).toBe(true);
    for(const x of [-14,14]) expect(visited.has(x+',-23')).toBe(true);
  }
});

it('hides only the occupied shell, includes the doorway, and restores outside',()=>{
  for(const seed of [0,1,2]) for(const b of stationBuildings(seed)) {
    expect(roofHidden(seed,b.id,b.x,b.z)).toBe(true);
    expect(roofHidden(seed,b.id,b.x,b.z+3.4)).toBe(true);
    expect(roofHidden(seed,b.id,b.x,b.z+4.3)).toBe(false);
    for(const other of stationBuildings(seed).filter(r=>r.id!==b.id))
      expect(roofHidden(seed,other.id,b.x,b.z)).toBe(false);
  }
});
