import {it,expect} from 'vitest';
import {EXPLORATION_CAMERA as c,EXPLORATION_OFFSET as p,aimLead} from '../../shared/exploration-camera.ts';
it('uses report pitch yaw distance and perspective FOV',()=>{expect(Math.hypot(...p)).toBeCloseTo(45);expect(Math.asin(p[1]/45)*180/Math.PI).toBeCloseTo(55);expect(Math.atan2(p[0],p[2])*180/Math.PI).toBeCloseTo(-30);expect(c.fov).toBe(20);});
it('centred cursor produces zero lookahead and offset is bounded at five metres',()=>{expect(aimLead(0,0)).toEqual({x:0,z:0});expect(Math.hypot(...Object.values(aimLead(100,-100)))).toBeCloseTo(5);expect(aimLead(1,2)).toEqual({x:1,z:2});});
