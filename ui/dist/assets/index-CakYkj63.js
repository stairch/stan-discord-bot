import{a as i}from"./announcements-DHD58ULG.js";import{f as h,l as m,A as f,r as v,o as _,c as p,a as s,t as l,i as g,e as c,b as y,w as b,F as S,p as U,d as k,_ as w}from"./index-DC9IV7qu.js";const e=n=>(U("data-v-3ec9129d"),n=n(),k(),n),x=e(()=>s("h1",null,"User Management",-1)),C={class:"user-management"},D={key:0,class:"stats"},E={class:"card"},I=e(()=>s("p",null,"HSLU-I Students",-1)),H={class:"card"},L=e(()=>s("p",null,"Students on Discord",-1)),M={class:"card"},N=e(()=>s("p",null,"Graduates on Discord",-1)),V=e(()=>s("h2",null,"Configuration",-1)),B={class:"config"},F={class:"config-option"},$=e(()=>s("div",{class:"info"},[s("h3",null,"Update Students"),s("p",null,"Update the list of students that is provided by the HSLU administration.")],-1)),A=e(()=>s("span",{class:"material-symbols-rounded"},"file_upload",-1)),G={class:"config-option"},P=e(()=>s("div",{class:"info"},[s("h3",null,"Update Modules"),s("p",null,"Update the list of modules that is provided by the HSLU administration.")],-1)),T=e(()=>s("span",{class:"material-symbols-rounded"},"file_upload",-1)),j={class:"config-option"},q=e(()=>s("div",{class:"info"},[s("h3",null,"Edit Degree Programmes"),s("p",null,"Edit the mapping between the HSLU degree programmes and the Discord roles & channels.")],-1)),z=e(()=>s("button",{class:"secondary"},[s("span",{class:"material-symbols-rounded"},"edit"),c(" Edit ")],-1)),J=h({__name:"index",setup(n){const o=m(null),r=async d=>{const t=document.createElement("input");t.type="file",t.accept=".csv",t.onchange=async()=>{if(!t.files||t.files.length===0)return;const a=await t.files[0].text();d==="students"?await i.db.updateStudents(a):d==="modules"&&await i.db.updateModules(a)},t.click()};return f(async()=>{o.value=await i.db.students()}),(d,t)=>{const u=v("router-link");return _(),p(S,null,[x,s("div",C,[o.value?(_(),p("div",D,[s("div",E,[s("h1",null,l(o.value.enrolled),1),I]),s("div",H,[s("h1",null,l(o.value.discord.students),1),L]),s("div",M,[s("h1",null,l(o.value.discord.graduates),1),N])])):g("",!0),V,s("div",B,[s("div",F,[$,s("button",{class:"secondary",onClick:t[0]||(t[0]=a=>r("students"))},[A,c(" Update ")])]),s("div",G,[P,s("button",{class:"secondary",onClick:t[1]||(t[1]=a=>r("modules"))},[T,c(" Update ")])]),s("div",j,[q,y(u,{to:"/discord/degree-programmes",class:"secondary"},{default:b(()=>[z]),_:1})])])])],64)}}}),Q=w(J,[["__scopeId","data-v-3ec9129d"]]);export{Q as default};
