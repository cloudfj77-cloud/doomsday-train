import { describe, expect, it } from 'vitest';
import { sceneConfig as scene } from '../../scripts/config/game.ts';
import { advanceCamera, createCamera, groundPoint, regionParameters, validateCamera } from '../../scripts/logic/camera.ts';
const c=scene.camera;
describe('camera ground ray and region mixing',()=>{
  it('projects to foot plane and rejects unavailable, parallel and backwards rays',()=>{
    expect(groundPoint([0,10,0,1,-1,2],2)).toEqual([8,16]);
    for(const ray of [undefined,[0,0,0,1,0,0],[0,10,0,1,1,0],[0,0,0,NaN,-1,0]]) expect(groundPoint(ray,0)).toBeUndefined();
  });
  it('validates complete parameters and unique region identities',()=>{
    validateCamera(c,scene.zones);
    for(const config of [{...c,distance:0},{...c,yaw:0},{...c,fov:180},{...c,followTime:-1}])expect(()=>validateCamera(config,scene.zones)).toThrow();
    for(const zones of [[scene.zones[0],scene.zones[0]],[{...scene.zones[0],blend:0}],[{...scene.zones[0],camera:{...scene.zones[0].camera,pitch:100}}]])expect(()=>validateCamera(c,zones)).toThrow();
  });
  it('blends region edges, equal priorities and higher-priority coverage',()=>{
    expect(regionParameters([20,0,20],c,scene.zones).parameters.distance).toBe(45);
    expect(regionParameters([3,0,-3],c,scene.zones).parameters.distance).toBe(38);
    expect(regionParameters([4.5,0,-3],c,scene.zones).parameters.distance).toBe(41.5);
    const second={...scene.zones[0],id:'second',camera:{...scene.zones[0].camera,distance:30}};
    expect(regionParameters([3,0,-3],c,[scene.zones[0],second]).parameters.distance).toBe(34);
    expect(regionParameters([3,0,-3],c,[scene.zones[0],{...second,priority:2}]).parameters.distance).toBe(30);
  });
});
describe('camera time response',()=>{
  it('initializes at the configured arm and converges without overshoot',()=>{
    const initial=createCamera([0,0,0],c,[]);
    expect(Math.hypot(...initial.position)).toBeCloseTo(45);
    expect(initial.position[0]).toBeLessThan(0);expect(initial.position[1]).toBeGreaterThan(30);
    let view=initial;
    for(let i=0;i<120;i++)view=advanceCamera(view,[2,1,2],[100,100],1/60,c,[]);
    expect(Math.hypot(...view.offset)).toBeCloseTo(5);
    expect(view.follow[0]).toBeCloseTo(2);expect(view.follow[1]).toBeCloseTo(1);
    const cleared=advanceCamera(view,[2,1,2],undefined,1,c,[]);
    expect(Math.hypot(...cleared.offset)).toBeLessThan(.01);
  });
  it('is frame-rate independent for a fixed target and smoothly returns from zones',()=>{
    const simulate=(fps:number)=>{let v=createCamera([0,0,0],c,[]);for(let i=0;i<fps;i++)v=advanceCamera(v,[2,0,2],[20,2],1/fps,c,[]);return v;};
    expect(simulate(30).target[0]).toBeCloseTo(simulate(120).target[0],8);
    let v=createCamera([3,0,-3],c,scene.zones);
    v=advanceCamera(v,[20,0,20],undefined,.1,c,scene.zones);
    expect(v.parameters.distance).toBeGreaterThan(38);expect(v.parameters.distance).toBeLessThan(45);
    for(let i=0;i<180;i++)v=advanceCamera(v,[20,0,20],undefined,1/60,c,scene.zones);
    expect(v.parameters.distance).toBeCloseTo(45,3);
  });
  it('does not amplify its own pan when a near-centre cursor stays still',()=>{
    let v=createCamera([0,0,0],c,[]);
    for(let i=0;i<600;i++)v=advanceCamera(v,[0,0,0],[.2+v.offset[0],v.offset[1]],1/60,c,[]);
    expect(v.offset[0]).toBeCloseTo(.2,5);expect(v.offset[1]).toBeCloseTo(0);
  });
});
