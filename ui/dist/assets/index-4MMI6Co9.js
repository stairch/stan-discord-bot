import{a as c}from"./announcements-2JhaBhjc.js";import{L as w}from"./LoadingWithResultModal-ByudaJxP.js";import{f as x,m as S,n as C,r as E,o as b,c as U,b as k,a as s,t as r,i as L,e as u,w as M,F as D,p as I,d as N,_ as H}from"./index-DdA6ahaw.js";const t=d=>(I("data-v-54e2e0c1"),d=d(),N(),d),V=t(()=>s("h1",null,"User Management",-1)),B={class:"user-management"},T={key:0,class:"stats"},F={class:"card"},$=t(()=>s("p",null,"HSLU-I Students",-1)),G={class:"card"},P=t(()=>s("p",null,"Students on Discord",-1)),R={class:"card"},W=t(()=>s("p",null,"Graduates on Discord",-1)),j=t(()=>s("h2",null,"Configuration",-1)),q={class:"config"},z={class:"config-option"},A=t(()=>s("div",{class:"info"},[s("h3",null,"Update Students"),s("p",null," Update the list of students that is provided by the HSLU administration. ")],-1)),J=t(()=>s("span",{class:"material-symbols-rounded"},"file_upload",-1)),K={class:"config-option"},O=t(()=>s("div",{class:"info"},[s("h3",null,"Update Modules"),s("p",null," Update the list of modules that is provided by the HSLU administration. ")],-1)),Q=t(()=>s("span",{class:"material-symbols-rounded"},"file_upload",-1)),X={class:"config-option"},Y=t(()=>s("div",{class:"info"},[s("h3",null,"Edit Degree Programmes"),s("p",null," Edit the mapping between the HSLU degree programmes and the Discord roles & channels. ")],-1)),Z=t(()=>s("button",{class:"secondary"},[s("span",{class:"material-symbols-rounded"},"edit"),u(" Edit ")],-1)),ss=x({__name:"index",setup(d){const o=S(null),n=S(null),p=async i=>{const e=document.createElement("input");e.type="file",e.accept=".csv",e.onchange=async()=>{var m,f,v,g,y;if((m=o.value)==null||m.onLoading(),!e.files||e.files.length===0)return;const a=await e.files[0].text(),h=a.split(`
`)[0];let l=null;if(i==="students"){if(!h.includes("Nachname")){(f=o.value)==null||f.onError("This format is unsupported.");return}l=await c.db.updateStudents(a)}else if(i==="modules"){if(!h.includes("Modultyp")){(v=o.value)==null||v.onError("This format is unsupported.");return}l=await c.db.updateModules(a)}l?(y=o.value)==null||y.onError(l):(g=o.value)==null||g.onSuccess("Successfully updated!")},e.click()};return C(async()=>{n.value=await c.db.students()}),(i,e)=>{const _=E("router-link");return b(),U(D,null,[k(w,{ref_key:"modal",ref:o},null,512),V,s("div",B,[n.value?(b(),U("div",T,[s("div",F,[s("h1",null,r(n.value.enrolled),1),$]),s("div",G,[s("h1",null,r(n.value.discord.students),1),P]),s("div",R,[s("h1",null,r(n.value.discord.graduates),1),W])])):L("",!0),j,s("div",q,[s("div",z,[A,s("button",{class:"secondary",onClick:e[0]||(e[0]=a=>p("students"))},[J,u(" Update ")])]),s("div",K,[O,s("button",{class:"secondary",onClick:e[1]||(e[1]=a=>p("modules"))},[Q,u(" Update ")])]),s("div",X,[Y,k(_,{to:"/discord/degree-programmes",class:"secondary"},{default:M(()=>[Z]),_:1})])])])],64)}}}),ns=H(ss,[["__scopeId","data-v-54e2e0c1"]]);export{ns as default};