<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container-fluid">
            <a class="navbar-brand" href="/" draggable="false">
                <font-awesome-icon icon="fa-solid fa-cube" class="me-2"/>
                <span class="me-2">mybacklog.io</span>
                <span v-if="isdemo" class="text-warning">demo</span>
            </a>
            <button class="navbar-toggler" type="button" @click="toggleNavbar = !toggleNavbar" data-toggle="collapse" data-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div id="navbarNav" class="navbar-collapse" :style="[ toggleNavbar ? null : { display:'none' } ]">
                <ul class="navbar-nav me-auto mb-2 mb-lg-0">
                    <li class="nav-item">
                        <a class="nav-link" href="/Menu/About">About</a>
                    </li>                    
                </ul>
                <div class="d-flex">
                    <div v-if="isauth" class="ms-auto">
                        <div class="btn-group">
                            <button class="btn dropdown-toggle btn-secondary p-2" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-user" size="xl" />
                            </button>
                            <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="dropdownMenuButton">
                                <li>
                                    <a href="/UserSettings" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-gear" /><span class="ps-2">Settings</span></a>
                                </li>
                                <li>
                                    <a href="/Logout" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-right-from-bracket" /><span class="ps-2">Log out</span></a>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <ul v-else-if="!isdemo" class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link" href="/Login">Log In</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="/SignUp">Sign Up</a>
                        </li>  
                        <li class="nav-item">
                            <a class="nav-link" :href="demourl">Demo</a>
                        </li>                                      
                    </ul>
                </div>
            </div>
        </div>                        
    </nav>       
</template>
<script>
    import axios from 'axios'
    import { successToast, errorToast, getCookie, listenCookieChange } from '../../js/common';
    
    export default {
        name: "Navbar",
        props: {
            isauth: Boolean,
            username: String,
            userid: String,
            isdemo: Boolean,
            demourl: String
        },
        data: function () {
            return {
                toggleNavbar: false
            }
        },       
        computed: {
        },
        created: function () {
        },         
        mounted() {
            var that = this;         
            var importingUserAccountIDs = JSON.parse(sessionStorage.getItem('importingUserAccountIDs')) ?? {};
            
            if (Object.keys(importingUserAccountIDs).length > 0) {
                var intervalID = setInterval(function() {
                    axios.get('/Game/GetCompletedImportGames')
                        .then(res => {
                            var results = res.data;

                            if (results.length > 0) {
                                Object.keys(importingUserAccountIDs).forEach(userAccountID => {
                                    var result = results.find(i => i.userAccountID == userAccountID);
                                    if (result) {
                                        if (result.success) {
                                            successToast("Imported <strong>" + result.count + "</strong> new games into <strong>" + importingUserAccountIDs[userAccountID].userListName + "<strong>");
                                        } else {
                                            result.errorMessages.forEach(errorMsg => {
                                                errorToast(errorMsg);                                         
                                            });
                                        }
                                        
                                        delete importingUserAccountIDs[userAccountID];
                                    }        
                                });

                                sessionStorage.setItem('importingUserAccountIDs', JSON.stringify(importingUserAccountIDs));
                                window.dispatchEvent(new CustomEvent('importingUserAccountIDsUpdate'));
                            }
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
                }, 10000);
            }
        },        
        methods: {                    
        }
    };
</script>






