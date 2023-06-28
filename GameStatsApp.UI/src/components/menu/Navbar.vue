<template>
    <nav class="navbar navbar-expand-lg navbar-light bg-light">
        <div class="container-fluid">
            <a class="navbar-brand" href="/" draggable="false">
                <font-awesome-icon icon="fa-solid fa-gear" />
                GameStatsApp
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
                    <autocomplete v-model="searchText" class="me-2" @change="onChange" @search="onSearch" @selected="onSearchSelected" :options="searchResults" labelby="label" valueby="label" :isasync="true" :loading="searchLoading" :placeholder="'Search users'"/>                
                    <div v-if="isauth">
                        <div class="btn-group">
                            <button class="btn dropdown-toggle btn-secondary" type="button" data-bs-toggle="dropdown" data-bs-display="static" aria-expanded="false">
                                <font-awesome-icon icon="fa-solid fa-user" />
                            </button>
                            <ul class="dropdown-menu dropdown-menu-end" aria-labelledby="dropdownMenuButton">
                                <li>
                                    <div class="dropdown-item">
                                        <div class="form-check form-switch">
                                            <input id="chkNightMode" class="form-check-input" type="checkbox" data-toggle="toggle" v-model="isDarkTheme">
                                            <label class="form-check-label" for="chkNightMode"><font-awesome-icon icon="fa-solid fa-moon" /><span class="ps-2">Night Mode</span></label>
                                        </div>
                                    </div>
                                </li>
                                <li>
                                    <a href="/User/UserSettings" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-gear" /><span class="ps-2">Settings</span></a>
                                </li>
                                <li>
                                    <a href="/Home/Logout" class="dropdown-item"><font-awesome-icon icon="fa-solid fa-right-from-bracket" /><span class="ps-2">Log out</span></a>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <ul v-else class="navbar-nav">
                        <li class="nav-item">
                            <a class="nav-link" href="/Home/Login">Log In</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" href="/Home/SignUp">Sign Up</a>
                        </li>                  
                    </ul>
                </div>
            </div>
        </div> 
    </nav>    
</template>
<script>
    import axios from 'axios'
    import { setCookie } from '../../js/common';

    export default {
        name: "Navbar",
        props: {
            isauth: Boolean,
            isdarktheme: Boolean,
            username: String,
            userid: String
        },
        data: function () {
            return {
                searchText: null,
                searchResults: [],
                searchLoading: false,
                showLoginModal: false,
                showResetModal: false,
                showSignUpModal: false,
                showDropdown: false,
                toggleNavbar: false,
                isDarkTheme: this.isdarktheme,
                state: false
            }
        },
        computed: {
        },
        watch: {
            isDarkTheme: function (val, oldVal) {
                var that = this;

                if (this.isauth) {
                    axios.post('/User/UpdateIsDarkTheme', null,{ params: { isDarkTheme: val } })
                        .then((res) => {
                            if (res.data.success) {
                                that.updateTheme(val);
                            }                                                                                   
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });        
                } else {
                    this.updateTheme(val);
                    var theme = val ? "theme-dark" : "theme-light";
                    setCookie("theme", theme);                  
                }
            }
        },
        created: function () {
        },        
        methods: {
            onInput: function(e){
                this.searchText = e;
            },
            onChange: function() {
                var a = this.searchText;
            },
            onSearch: function() {
                var that = this;
                this.searchLoading = true;
                               
                axios.get('/Menu/Search', { params: { term: this.searchText } })
                        .then(res => {
                            that.searchResults = res.data.reduce((flat, groupheader) => {
                                return flat
                                    .concat({
                                        label: groupheader.label,
                                        value: groupheader.subItems.map(method => method.value),
                                        isGroupHeader: true,
                                        disabled: true
                                    })
                                    .concat(groupheader.subItems.map(method => ({ label: method.label, value: method.value, category: groupheader.label })))
                            }, []);

                            if(that.searchResults.length == 0)
                            {
                                var noResult = { value: "", label: "No results found", category: null, disabled: true };
                                that.searchResults.push(noResult);
                            }

                            that.searchLoading = false;
                            return res;
                        })
                        .catch(err => { console.error(err); return Promise.reject(err); });
            },              
            onSearchSelected: function (result) {
                var controller;
                var action;

                if (result.category == 'Users') {
                    controller = "User";
                    action = "UserDetails"
                }
                
                location.href = encodeURI('/' + controller + "/" + action + "/" + result.value);
            },
            updateTheme: function(val){
                var el = document.body;

                if (val){
                    el.classList.remove("theme-light");
                    el.classList.add("theme-dark");
                } else {
                    el.classList.remove("theme-dark");
                    el.classList.add("theme-light");
                }
            },
            toggleDropdown(e) {
                this.state = !this.state
            }
        }
    };
</script>






